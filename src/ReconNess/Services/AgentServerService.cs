using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentServerService"/>
    /// </summary>
    public class AgentServerService : Service<AgentServer>, IService<AgentServer>, IAgentServerService
    {
        private readonly IAgentsSettingService agentsSettingService;

        private AgentsSetting agentsSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="IAgentServerService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public AgentServerService(IUnitOfWork unitOfWork, IAgentsSettingService agentsSettingService)
            : base(unitOfWork)
        {
            this.agentsSettingService = agentsSettingService;
        }

        /// <inheritdoc/>
        public async Task<int> GetAgentAvailableServerAsync(CancellationToken cancellationToken = default)
        {
            this.agentsSetting = this.agentsSetting ?? (await this.agentsSettingService.GetAllAsync(cancellationToken)).FirstOrDefault();

            return 1;
        }
    }
}
