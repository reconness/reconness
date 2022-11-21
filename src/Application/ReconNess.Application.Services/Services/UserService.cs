using NLog;
using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IUserService"/>
    /// </summary>
    public class UserService : Service<User>, IService<User>, IUserService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="ILabelService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public UserService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
