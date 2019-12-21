using System.Collections.Generic;
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
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public static async Task<object> GenerateJwt(string userName, IList<Claim> claims, IJwtFactory jwtFactory, JwtIssuerOptions jwtOptions)
        {

            return new
            {
                userName,
                auth_token = await jwtFactory.GenerateEncodedToken(userName, claims),
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };
        }
    }
}
