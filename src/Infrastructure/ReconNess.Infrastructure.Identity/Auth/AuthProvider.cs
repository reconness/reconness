using Microsoft.AspNetCore.Http;
using ReconNess.Application.Providers;
using System.Security.Claims;

namespace ReconNess.Infrastructure.Identity.Auth;

/// <summary>
/// 
/// </summary>
public class AuthProvider : IAuthProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"><see cref="IHttpContextAccessor"/></param>
    public AuthProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string UserName()
    {
        return httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string[] Roles()
    {
        var roles = httpContextAccessor?.HttpContext?.User?.Claims?.Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType)?.FirstOrDefault()?.Value ?? string.Empty;

        if (roles.Contains(","))
        {
            return roles.Split(",");
        }

        return new string[] { roles };
    }

    public bool AreYouMember()
    {
        return Roles().Contains("Member");
    }

    public bool AreYouAdmin()
    {
        return Roles().Contains("Admin");
    }

    public bool AreYouOwner()
    {
        var onwer = httpContextAccessor?.HttpContext?.User.Claims.Where(c => c.Type == "Owner")?.FirstOrDefault()?.Value ?? "false";
        return !string.IsNullOrEmpty(onwer) && bool.Parse(onwer);
    }
}
