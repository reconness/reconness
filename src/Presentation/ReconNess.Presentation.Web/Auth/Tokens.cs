using ReconNess.Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReconNess.Presentation.Api.Auth;

public class Tokens
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="roles"></param>
    /// <param name="jwtFactory"></param>
    /// <param name="jwtOptions"></param>
    /// <returns></returns>
    public static async Task<object> GenerateJwt(User user, IList<string> roles, IJwtFactory jwtFactory, JwtIssuerOptions jwtOptions)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName)
        };

        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(',', roles)));
        claims.Add(new Claim("Owner", user.Owner ? "true" : "false"));

        return new
        {
            user.UserName,
            roles,
            user.Owner,
            auth_token = await jwtFactory.GenerateEncodedToken(user.UserName, claims),
            expires_in = (int)jwtOptions.ValidFor.TotalSeconds
        };
    }
}
