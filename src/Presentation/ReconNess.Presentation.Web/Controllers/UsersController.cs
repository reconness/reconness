using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Application.Providers;
using ReconNess.Application.Services;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers;

[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IUserService userService;
    private readonly IRoleService roleService;
    private readonly IAuthProvider authProvider;
    private readonly UserManager<User> userManager;
    private readonly IEventTrackService eventTrackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController" /> class
    /// </summary>
    /// <param name="mapper"><see cref="IMapper"/></param>
    /// <param name="userService"><see cref="IUserService"/></param>
    /// <param name="roleService"><see cref="IRoleService"/></param>
    /// <param name="authProvider"><see cref="IAuthProvider"/></param>
    /// <param name="userManager"><see cref="UserManager{TUser}"/></param>
    /// <param name="eventTrackService"><see cref="IEventTrackService"/></param>
    public UsersController(IMapper mapper,
        IUserService userService,
        IRoleService roleService,
        IAuthProvider authProvider,
        UserManager<User> userManager,
        IEventTrackService eventTrackService)
    {
        this.mapper = mapper;
        this.userService = userService;
        this.roleService = roleService;
        this.authProvider = authProvider;
        this.userManager = userManager;
        this.eventTrackService = eventTrackService;
    }

    /// <summary>
    /// Obtain the list of users.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/users
    ///
    /// </remarks>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of targets</returns>
    /// <response code="200">Returns the list of users</response>
    /// <response code="401">If the user is not authenticate or admin role</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var users = new List<User>();

        if (this.authProvider.AreYouOwner())
        {
            // get all users
            users = await this.userService
                        .GetAllQueryable()
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);
        }
        else if (this.authProvider.AreYouMember())
        {
            // get yourself only
            users.Add(await this.userService
                    .GetAllQueryableByCriteria(u => u.UserName == this.authProvider.UserName())
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken));
        }
        else if (this.authProvider.AreYouAdmin())
        {
            // get yourself and member users only
            var allUsers = await this.userService
                        .GetAllQueryable()
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

            foreach (var user in allUsers)
            {
                if (user.UserName == this.authProvider.UserName() || await this.userManager.IsInRoleAsync(user, "Member"))
                {
                    users.Add(user);
                }
            }
        }

        return Ok(this.mapper.Map<List<User>, List<UserDto>>(users));
    }

    /// <summary>
    /// Obtain a user by name.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/users/{id}
    ///
    /// </remarks>
    /// <param name="id">The user id</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A target by name</returns>
    /// <response code="200">Returns a user by id</response>
    /// <response code="400">Bad Request</response>
    /// <response code="401">If the user is not authenticate or is not admin</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var user = await userService.GetByCriteriaAsync(u => u.Id == id, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }

        var currentUserName = this.authProvider.UserName();
        if (this.authProvider.AreYouMember() && user.UserName != currentUserName)
        {
            return BadRequest("You only can see your own user.");
        }

        if (this.authProvider.AreYouAdmin() && user.UserName != currentUserName && !(await this.userManager.IsInRoleAsync(user, "Member")))
        {
            return BadRequest("You only can see your own user or member users.");
        }

        return Ok(mapper.Map<User, UserDto>(user));
    }

    /// <summary>
    /// Save a new user.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST api/users
    ///     {
    ///         "username": "username",
    ///         "email": "myemail@mydomain.com",
    ///         "firstName": "firstName",
    ///         "lastName": "lastName",
    ///         "newPassword": "newPassword",
    ///         "confirmationPassword": "confirmationPassword",
    ///         "role": "[admin, member]"
    ///     }
    ///
    /// </remarks>
    /// <param name="userDto">The user dto</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <response code="200">The new user</response>
    /// <response code="400">Bad Request</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Post([FromBody] UserDto userDto, CancellationToken cancellationToken)
    {
        // only the Owner and the Admin user can add new users
        if (authProvider.AreYouMember())
        {
            return BadRequest("You can not add a new user.");
        }

        if (string.IsNullOrWhiteSpace(userDto.NewPassword))
        {
            return BadRequest("Password is required.");
        }

        if (!this.authProvider.AreYouOwner() && this.authProvider.AreYouAdmin() && "Admin".Equals(userDto.Role))
        {
            return BadRequest("You can not add Admin new user.");
        }

        if (!"Member".Equals(userDto.Role) && !"Admin".Equals(userDto.Role))
        {
            return BadRequest("You can not add an invalid Role.");
        }

        var userExist = await this.userService.AnyAsync(t => t.UserName.ToLower() == userDto.UserName.ToLower(), cancellationToken);
        if (userExist)
        {
            return BadRequest("A user with that user name exist");
        }            

        var user = mapper.Map<UserDto, User>(userDto);

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status409Conflict, result.Errors);
        }

        await userManager.AddPasswordAsync(user, userDto.NewPassword);
        await userManager.AddToRolesAsync(user, new List<string> { userDto.Role });
        await userManager.AddClaimsAsync(user, GetClaims(userDto.Role));

        await this.eventTrackService.AddAsync(new EventTrack
        {
            Description = $"User {user.Email} Added"
        }, cancellationToken);

        return Ok(user);
    }

    /// <summary>
    /// Update an user.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT api/users/{id}
    ///     {
    ///         "username": "username",
    ///         "email": "myemail@mydomain.com",
    ///         "firstName": "firstName",
    ///         "lastName": "lastName",
    ///         "currentPassword": "currentPassword"
    ///         "newPassword": "newPassword",
    ///         "confirmationPassword": "confirmationPassword"
    ///         "role": "[admin, member]"
    ///     }
    ///
    /// </remarks>
    /// <param name="id">The user id</param>
    /// <param name="userDto">The user dto</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <response code="204">No Content</response>
    /// <response code="400">Bad Request</response>
    /// <response code="401">If the user is not authenticate</response>
    /// <response code="404">Not Found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UserDto userDto, CancellationToken cancellationToken)
    {
        if (!"Member".Equals(userDto.Role) && !"Admin".Equals(userDto.Role))
        {
            return BadRequest("You can not add an invalid Role.");
        }

        var user = await this.userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        var currentUserName = this.authProvider.UserName();
        if (!this.authProvider.AreYouOwner() && this.authProvider.AreYouMember() && !user.UserName.Equals(currentUserName))
        {
            return BadRequest("You can only edit yourself.");
        }

        if (!this.authProvider.AreYouOwner() && this.authProvider.AreYouAdmin() && !user.UserName.Equals(currentUserName) && !(await this.userManager.IsInRoleAsync(user, "Member")))
        {
            return BadRequest("You only can edit yourself or a Member users.");
        }

        if (user.UserName != userDto.UserName && await this.userService.AnyAsync(t => t.UserName == userDto.UserName, cancellationToken))
        {
            return BadRequest("A user with that user name exist");
        }

        // if only need a current password when you are trying to change the password and the user is yourself
        if (!string.IsNullOrWhiteSpace(userDto.NewPassword) && user.UserName.Equals(currentUserName))
        {
            if (string.IsNullOrWhiteSpace(userDto.CurrentPassword) || !await userManager.CheckPasswordAsync(user, userDto.CurrentPassword))
            {
                return BadRequest("Current password is not valid.");
            }
        }

        user.UserName = userDto.UserName;
        user.Email = userDto.Email;
        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.Image = userDto.Image;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        if (!string.IsNullOrWhiteSpace(userDto.NewPassword))
        {
            await userManager.RemovePasswordAsync(user);
            await userManager.AddPasswordAsync(user, userDto.NewPassword);
        }
                    
        await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
        await userManager.RemoveClaimsAsync(user, await userManager.GetClaimsAsync(user));

        await userManager.AddToRolesAsync(user, new List<string> { userDto.Role });
        await userManager.AddClaimsAsync(user, GetClaims(userDto.Role));

        await this.eventTrackService.AddAsync(new EventTrack
        {
            Description = $"User {user.Email} edited"
        }, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete a user.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE api/users/{id}
    ///
    /// </remarks>
    /// <param name="id">The user id</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <response code="204">No Content</response>
    /// <response code="400">Bad Request</response>
    /// <response code="401">If the user is not authenticate or is not admin</response>
    /// <response code="404">Not Found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (this.authProvider.AreYouMember())
        {
            return BadRequest("You can not remove an user.");
        }

        var user = await this.userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        if (user.UserName.Equals(this.authProvider.UserName()))
        {
            return BadRequest("You can not remove your own user.");
        }

        if (this.authProvider.AreYouAdmin() && !(await this.userManager.IsInRoleAsync(user, "Member")))
        {
            return BadRequest("You only can remove Member users.");
        }            

        await userManager.DeleteAsync(user);

        await this.eventTrackService.AddAsync(new EventTrack
        {
            Description = $"User {user.Email} deleted"
        }, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Obtain the list of roles.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/users
    ///
    /// </remarks>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of targets</returns>
    /// <response code="200">Returns the list of users</response>
    /// <response code="401">If the user is not authenticate or admin role</response>
    [HttpGet("roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
    {
        var roles = await this.roleService
                    .GetAllQueryable()
                    .AsNoTracking()
                    .Select(r => r.Name)
                    .ToListAsync(cancellationToken);

        return Ok(roles);
    }

    /// <summary>
    /// Update an user the Owner.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT api/users/assignOnwer/{id}
    ///     {
    ///     }
    ///
    /// </remarks>
    /// <param name="id">The user id</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <response code="204">No Content</response>
    /// <response code="400">Bad Request</response>
    /// <response code="401">If the user is not authenticate</response>
    /// <response code="404">Not Found</response>
    [HttpPut("assignOwner/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignOwner(Guid id, CancellationToken cancellationToken)
    {
        if (!this.authProvider.AreYouOwner())
        {
            return BadRequest("You can not assign owner.");
        }

        var user = await this.userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        if (user.UserName.Equals(this.authProvider.UserName()))
        {
            return BadRequest("You can not reassign owner to yourself.");
        }

        if (!(await this.userManager.IsInRoleAsync(user, "Admin")))
        {
            return BadRequest("You only can assign owner to an Admin user.");
        }

        user.Owner = true;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var currentUser = await this.userService.GetByCriteriaAsync(u => u.UserName == this.authProvider.UserName() && u.Owner, cancellationToken);
        currentUser.Owner = false;

        await userService.UpdateAsync(currentUser, cancellationToken);

        await this.eventTrackService.AddAsync(new EventTrack
        {
            Description = $"User {user.Email} assigned as new Owner"
        }, cancellationToken);

        return NoContent();
    }
    
    /// <summary>
    /// Obtain the role claim
    /// </summary>
    /// <param name="role">The role [Admin, Member]</param>
    /// <returns></returns>
    private static IEnumerable<Claim> GetClaims(string role)
    {
        var claims = new List<Claim>();

        if (role.ToLower() == "admin")
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }
        else if (role.ToLower() == "member")
        {
            claims.Add(new Claim(ClaimTypes.Role, "Member"));
        }

        return claims;
    }
}
