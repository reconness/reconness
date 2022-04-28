using Microsoft.Extensions.DependencyInjection;
using ReconNess.Core.Managers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Managers
{
    public class AgentServerSetting : IAgentServerSetting
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="IAgentServerSetting" /> class
        /// </summary>
        /// <param name="serviceScopeFactory"><see cref="IServiceScopeFactory"/></param>
        public AgentServerSetting(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<AgentsSetting> GetAgentSettingAsync(CancellationToken cancellationToken = default)
        {
            using var scope = this.serviceScopeFactory.CreateScope();
            var agentsSettingService = scope.ServiceProvider.GetService<IAgentsSettingService>();

            return (await agentsSettingService.GetAllAsync(cancellationToken)).FirstOrDefault();
        }
    }
}
