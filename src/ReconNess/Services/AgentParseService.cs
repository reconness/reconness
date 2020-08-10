using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ReconNess.Core.Models;
using ReconNess.Core.Services;

namespace ReconNess.Services
{
    public class AgentParseService : IAgentParseService
    {
        private readonly IServiceProvider serviceProvider;
        public AgentParseService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task SaveScriptOutputAsync(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var rootDomainService =
                    scope.ServiceProvider
                        .GetRequiredService<IRootDomainService>();

                await rootDomainService.SaveScriptOutputAsync(agentRun, scriptOutput, cancellationToken);
            }
        }
    }
}