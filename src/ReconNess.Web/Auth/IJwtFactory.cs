using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReconNess.Web.Auth
{
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
        Task<string> GenerateEncodedToken(string userName, IList<Claim> claims);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        ClaimsIdentity GenerateClaimsIdentity(string userName, IList<Claim> claims);
    }
}
