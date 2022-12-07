using System.Security.Claims;

namespace ReconNess.Infrastructure.Identity.Auth;

/// <summary>
/// 
/// </summary>
public interface IJwtFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="claims"></param>
    /// <returns></returns>
    Task<string> GenerateEncodedToken(string userName, IEnumerable<Claim> claims);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="claims"></param>
    /// <returns></returns>
    ClaimsIdentity GenerateClaimsIdentity(string userName, IEnumerable<Claim> claims);
}
