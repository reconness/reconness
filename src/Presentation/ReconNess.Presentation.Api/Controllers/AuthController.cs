using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ReconNess.Infrastructure.Identity.Auth;
using ReconNess.Infrastructure.Identity.Entities;
using ReconNess.Presentation.Api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReconNess.Presentation.Api.Controllers;

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
    /// <returns>The JWT</returns>
    /// <response code="200">Returns the JWT</response>
    /// <response code="400">Bad Request if the credentials are not correct</response>
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] CredentialsViewModel credentials)
    {
        var defaultUserExistOrWasCreated = await CheckOrAddDefaultUser();
        if (!defaultUserExistOrWasCreated)
        {
            var errorMsg = "We had an issue creating the default User, please check if you have 'ReconnessUserName' and 'ReconnessPassword' in your local environment variables";
            return StatusCode(StatusCodes.Status500InternalServerError, errorMsg);
        }

        var user = await SingInAsync(credentials.UserName, credentials.Password);
        if (user == null)
        {
            return BadRequest("Invalid username or password.");
        }

        var roles = await signInManager.UserManager.GetRolesAsync(user);

        var jwt = await Tokens.GenerateJwt(
            user,
            roles,
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
        if (signInManager.UserManager.Users.Any())
        {
            return true;
        }

        var envUserName = Environment.GetEnvironmentVariable("ReconnessUserName") ??
                          Environment.GetEnvironmentVariable("ReconnessUserName", EnvironmentVariableTarget.User);

        var envPassword = Environment.GetEnvironmentVariable("ReconnessPassword") ??
                          Environment.GetEnvironmentVariable("ReconnessPassword", EnvironmentVariableTarget.User);

        var envEmail = Environment.GetEnvironmentVariable("ReconnessEmail") ??
                          Environment.GetEnvironmentVariable("ReconnessEmail", EnvironmentVariableTarget.User);

        if (string.IsNullOrEmpty(envUserName) || string.IsNullOrEmpty(envPassword))
        {
            return false;
        }

        var user = new User { UserName = envUserName, Email = envEmail, Owner = true };

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
