using NLog;
using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IServiceService"/>
    /// </summary>
    public class ServiceService : Service<Service>, IService<Service>, IServiceService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public ServiceService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
