using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentsSettingService"/>
    /// </summary>
    public class AgentsSettingService : Service<AgentsSetting>, IService<AgentsSetting>, IAgentsSettingService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IAgentsSettingService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public AgentsSettingService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
