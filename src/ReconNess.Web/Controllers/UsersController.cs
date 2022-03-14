using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static readonly string ERROR_USER_EXIT = "A user with that user name exist";

        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IRoleService roleService;
        private readonly IAuthProvider authProvider;
        private readonly UserManager<User> userManager;

        public UsersController(IMapper mapper,
            IUserService userService,
            IRoleService roleService,
            IAuthProvider authProvider,
            UserManager<User> userManager)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.roleService = roleService;
            this.authProvider = authProvider;
            this.userManager = userManager;
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
            var users = await this.userService
                        .GetAllQueryable()
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

            return Ok(this.mapper.Map<List<User>, List<UserDto>>(users));
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
            var user = await this.userService.GetByCriteriaAsync(u => u.Id == id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            // only the Owner and the Admin user can see the into of the other users
            if (!this.authProvider.AreYouAdmin() && !this.authProvider.AreYouOwner())
            {
                if (!user.UserName.Equals(this.authProvider.UserName()))
                {
                    return BadRequest("You only can see your own user.");
                }
            }

            return Ok(this.mapper.Map<User, UserDto>(user));
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
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] UserDto userDto, CancellationToken cancellationToken)
        {
            // only the Owner and the Admin user can add new users
            if (this.authProvider.AreYouMember())
            {
                return BadRequest("You can not add a new user.");
            }

            if (string.IsNullOrWhiteSpace(userDto.NewPassword))
            {
                return BadRequest("Password is required.");
            }

            var userExist = await this.userService.AnyAsync(t => t.UserName.ToLower() == userDto.UserName.ToLower(), cancellationToken);
            if (userExist)
            {
                return BadRequest(ERROR_USER_EXIT);
            }

            var user = this.mapper.Map<UserDto, User>(userDto);

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status409Conflict, result.Errors);
            }

            await userManager.AddPasswordAsync(user, userDto.NewPassword);
            await userManager.AddToRolesAsync(user, new List<string> { userDto.Role });
            await userManager.AddClaimsAsync(user, GetClaims(userDto.Role));

            return NoContent();
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
            var user = await this.userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            if (!await this.CanEditAsync(user))
            {
                return BadRequest("You can not edit this user.");
            }

            if (user.UserName != userDto.UserName && await this.userService.AnyAsync(t => t.UserName == userDto.UserName, cancellationToken))
            {
                return BadRequest(ERROR_USER_EXIT);
            }

            // if you want to change your password you only need to enter the currentPassword if you are not the Onwer
            if (!string.IsNullOrWhiteSpace(userDto.NewPassword) && await this.NeedCurrentPassword(user))
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

            if (!this.authProvider.AreYouMember())
            {
                await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
                await userManager.RemoveClaimsAsync(user, await userManager.GetClaimsAsync(user));

                await userManager.AddToRolesAsync(user, new List<string> { userDto.Role });
                await userManager.AddClaimsAsync(user, GetClaims(userDto.Role));
            }

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
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate or is not admin</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await this.userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            if (!await this.CanDeleteAsync(user))
            {
                return BadRequest("You can not remove this user.");
            }

            if (user.UserName.Equals(this.authProvider.UserName()))
            {
                return BadRequest("You can not remove your own user.");
            }

            await this.userManager.DeleteAsync(user);

            return NoContent();
        }

        /// <summary>
        /// Update an user.
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
            var user = await this.userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var currentUser = await this.userService.GetByCriteriaAsync(u => u.UserName == this.authProvider.UserName(), cancellationToken);
            if (currentUser == null || !await this.CanAssignOwnerAsync(user))
            {
                return BadRequest("You can not assign owner to this user.");
            }

            user.Owner = true;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            currentUser.Owner = false;

            await this.userService.UpdateAsync(currentUser, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> CanEditAsync(User user)
        {
            var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            return this.authProvider.AreYouOwner() || (this.authProvider.AreYouAdmin() && userRole.Equals("Member")) || user.UserName.Equals(this.authProvider.UserName());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> CanAssignOwnerAsync(User user)
        {
            var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            return this.authProvider.AreYouOwner() && userRole.Equals("Admin") && !user.UserName.Equals(this.authProvider.UserName());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> NeedCurrentPassword(User user)
        {
            var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            return !(this.authProvider.AreYouOwner() || (this.authProvider.AreYouAdmin() && userRole.Equals("Member")));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> CanDeleteAsync(User user)
        {
            var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            return this.authProvider.AreYouOwner() || (this.authProvider.AreYouAdmin() && userRole.Equals("Member"));
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
}
