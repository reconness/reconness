using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController" /> class
        /// </summary>
        /// <param name="jwtFactory"><see cref="IJwtFactory"/></param>
        /// <param name="jwtOptions"><see cref="JwtIssuerOptions"/></param>
        public AuthController(
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions)
        {
            this.jwtFactory = jwtFactory;
            this.jwtOptions = jwtOptions.Value;
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
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid username or password.");
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
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            // get the user from active directory
            var user = SingInEnv(userName, password);
            if (user != null)
            {
                return await Task.FromResult(jwtFactory.GenerateClaimsIdentity(user.UserName, GetClaims(user)));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private List<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("userName", user.UserName.ToString()));

            return claims;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private User SingInEnv(string userName, string password)
        {
            var envUserName = Environment.GetEnvironmentVariable("ReconnessUserName") ??
                              Environment.GetEnvironmentVariable("ReconnessUserName", EnvironmentVariableTarget.User);

            var envPassword = Environment.GetEnvironmentVariable("ReconnessPassword") ??
                              Environment.GetEnvironmentVariable("ReconnessPassword", EnvironmentVariableTarget.User);

            if (userName.Equals(envUserName) && password.Equals(envPassword))
            {
                return new User
                {
                    UserName = userName
                };
            }

            return null;
        }
    }
}
