using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
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
        private readonly IAuthProvider authProvider;

        public UsersController(IMapper mapper, IUserService userService, IAuthProvider authProvider)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.authProvider = authProvider;
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
                        .GetAllQueryable(cancellationToken)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

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
            var user = await this.userService.GetByCriteriaAsync(u => u.Id == id, cancellationToken);
            if (user == null)
            {
                return NotFound();
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
        ///         "name": "targetname"
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
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userExist = await this.userService.AnyAsync(t => t.UserName.ToLower() == userDto.UserName.ToLower());
            if (userExist)
            {
                return BadRequest(ERROR_USER_EXIT);
            }

            var user = this.mapper.Map<UserDto, User>(userDto);

            await this.userService.AddAsync(user, cancellationToken);

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
        ///         "name": "targetname"
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
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await this.userService.GetByCriteriaAsync(t => t.Id == id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            if (user.UserName != userDto.UserName && await this.userService.AnyAsync(t => t.UserName == userDto.UserName))
            {
                return BadRequest(ERROR_USER_EXIT);
            }

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;

            await this.userService.UpdateAsync(user, cancellationToken);

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
            var user = await this.userService.GetByCriteriaAsync(u => u.Id == id, cancellationToken);
            
            if (user == null)
            {
                return NotFound();
            }

            if (user.UserName.Equals(this.authProvider.UserName()))
            {
                return BadRequest("You can not remove your own user");
            }

            await this.userService.DeleteAsync(user, cancellationToken);

            return NoContent();
        }
    }
}
