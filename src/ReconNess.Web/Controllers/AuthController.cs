using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ReconNess.Entities;
using ReconNess.Web.Auth;
using ReconNess.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtFactory jwtFactory;
        private readonly JwtIssuerOptions jwtOptions;
        private readonly SignInManager<User> signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController" /> class
        /// </summary>
        /// <param name="jwtFactory"><see cref="IJwtFactory"/></param>
        /// <param name="jwtOptions"><see cref="JwtIssuerOptions"/></param>
        /// <param name="signInManager"></param>
        public AuthController(
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            SignInManager<User> signInManager)
        {
            this.jwtFactory = jwtFactory;
            this.jwtOptions = jwtOptions.Value;
            this.signInManager = signInManager;
        }

        /// <summary>
        /// Do the login.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/auth/login
        ///     {
        ///         "userName": "myusername",
        ///         "password": "mypassword"
        ///     }
        ///
        /// </remarks>
        /// <param name="credentials">The credentials</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The JWT</returns>
        /// <response code="200">Returns the JWT</response>
        /// <response code="400">Bad Request if the credentials are not correct</response>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] CredentialsViewModel credentials, CancellationToken cancellationToken)
        {
            var defaultUserExistOrWasCreated = await this.CheckOrAddDefaultUser();
            if (!defaultUserExistOrWasCreated)
            {
                var errorMsg = "We had an issue creating the default User, please check if you have 'ReconnessUserName' and 'ReconnessPassword' in your local environment variables";
                return StatusCode(StatusCodes.Status500InternalServerError, errorMsg);
            }

            var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
            if (identity == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var jwt = await Tokens.GenerateJwt(
                credentials.UserName,
                identity.Claims.ToList(),
                jwtFactory,
                jwtOptions);

            return new OkObjectResult(jwt);
        }

        /// <summary>
        /// Check if the default user exist and create it if it is not
        /// </summary>
        /// <returns>If the user exist or was created succefully</returns>
        private async Task<bool> CheckOrAddDefaultUser()
        {
            if (signInManager.UserManager.Users.Count() != 0)
            {
                return true;
            }

            var envUserName = Environment.GetEnvironmentVariable("ReconnessUserName") ??
                              Environment.GetEnvironmentVariable("ReconnessUserName", EnvironmentVariableTarget.User);

            var envPassword = Environment.GetEnvironmentVariable("ReconnessPassword") ??
                              Environment.GetEnvironmentVariable("ReconnessPassword", EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(envUserName) || string.IsNullOrEmpty(envPassword))
            {
                return false;
            }

            User user = new User { UserName = envUserName, Email = $"{envUserName}@youremaildomainhere.com" };
            
            var result = await signInManager.UserManager.CreateAsync(user, envPassword);
            if (result.Succeeded)
            {
                user = await signInManager.UserManager.FindByNameAsync(envUserName);
                await signInManager.UserManager.AddToRoleAsync(user, "Admin");
            }

            return result.Succeeded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            // get the user from active directory
            var user = await this.SingInAsync(userName, password);
            if (user != null)
            {
                return jwtFactory.GenerateClaimsIdentity(user.UserName, await GetClaimsAsync(user));
            }

            // Credentials are invalid, or account doesn't exist
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<List<Claim>> GetClaimsAsync(User user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName.ToString()));

            var roles = await signInManager.UserManager.GetRolesAsync(user);
            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(',', roles)));

            return claims;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<User> SingInAsync(string userName, string password)
        {
            var result = await signInManager.PasswordSignInAsync(userName, password, false, false);
            if (result.Succeeded)
            {
                return await signInManager.UserManager.FindByNameAsync(userName);
            }

            return null;
        }
    }
}
