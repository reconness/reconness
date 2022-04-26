using NLog;
using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IRoleService"/>
    /// </summary>
    public class RoleService : Service<Role>, IService<Role>, IRoleService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="IRoleService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public RoleService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
