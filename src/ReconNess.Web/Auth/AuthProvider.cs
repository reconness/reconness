using Microsoft.AspNetCore.Http;
using ReconNess.Core.Providers;

namespace ReconNess.Web.Auth
{
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
            return httpContextAccessor.HttpContext.User.Identity.Name;
        }
    }
}
