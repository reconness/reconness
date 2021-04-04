using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReconNess.Web.Auth
{
    public class Tokens
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="claims"></param>
        /// <param name="jwtFactory"></param>
        /// <param name="jwtOptions"></param>
        /// <returns></returns>
        public static async Task<object> GenerateJwt(string userName, IEnumerable<Claim> claims, IJwtFactory jwtFactory, JwtIssuerOptions jwtOptions)
        {
            var roles = claims.Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).FirstOrDefault().Value ?? string.Empty;
            var owner = claims.Where(c => c.Type == "Owner")?.FirstOrDefault().Value ?? "false";

            return new
            {
                userName,
                roles,
                owner,
                auth_token = await jwtFactory.GenerateEncodedToken(userName, claims),
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };
        }
    }
}
