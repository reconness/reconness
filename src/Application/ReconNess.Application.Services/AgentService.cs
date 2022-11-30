using ReconNess.Application.DataAccess;
using ReconNess.Application.DataAccess.Repositories;
using ReconNess.Application.Models;
using ReconNess.Application.Providers;
using ReconNess.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentService"/>
    /// </summary>
    public class AgentService : Service<Agent>, IService<Agent>, IAgentService
    {
        private readonly IScriptEngineProvider scriptEngineService;
        private readonly IMarketplaceProvider marketplaceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="scriptEngineService"><see cref="IScriptEngineProvider"/></param>
        public AgentService(IUnitOfWork unitOfWork,
            IScriptEngineProvider scriptEngineService,
            IMarketplaceProvider marketplaceProvider)
            : base(unitOfWork)
        {
            this.scriptEngineService = scriptEngineService;
            this.marketplaceProvider = marketplaceProvider;
        }

        /// <inheritdoc/>
        public async Task<List<Agent>> GetAgentsAsync(CancellationToken cancellationToken = default) => 
            await UnitOfWork.Repository<IAgentRepository, Agent>().GetAgentsAsync(cancellationToken);

        /// <inheritdoc/>
        public async Task<Agent?> GetAgentAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default) => 
            await UnitOfWork.Repository<IAgentRepository, Agent>().GetAgentAsync(criteria, cancellationToken);

        /// <inheritdoc/>
        public async Task<Agent?> GetAgentWithCategoriesTriggerAndEventsAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default) => 
            await UnitOfWork.Repository<IAgentRepository, Agent>().GetAgentWithCategoriesTriggerAndEventsAsync(criteria, cancellationToken);

        /// <inheritdoc/>
        public async Task<Agent?> GetAgentToRunAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default) =>
            await UnitOfWork.Repository<IAgentRepository, Agent>().GetAgentToRunAsync(criteria, cancellationToken);

        /// <inheritdoc/>
        public async Task<List<AgentMarketplace>> GetMarketplaceAsync(CancellationToken cancellationToken = default)
        {
            var agentMarketplaces = await this.marketplaceProvider.GetAgentMarketplacesAsync(cancellationToken);
            if (agentMarketplaces == null)
            {
                return new List<AgentMarketplace>();
            }

            return agentMarketplaces.Agents;
        }

        /// <inheritdoc/>
        public async Task<string> GetScriptAsync(string scriptUrl, CancellationToken cancellationToken)
        {
            return await this.marketplaceProvider.GetScriptAsync(scriptUrl, cancellationToken);            
        }

        /// <inheritdoc/>
        public async Task<ScriptOutput> DebugAsync(string script, string terminalOutput, CancellationToken cancellationToken = default)
        {
            return await scriptEngineService.TerminalOutputParseAsync(script, terminalOutput, 0, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<string> ReadConfigurationFileAsync(string configurationFileName, CancellationToken cancellationToken)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                configurationFileName = configurationFileName.Replace(c.ToString(), "");
            }

            var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "configurations");
            var path = Path.Combine(configPath, configurationFileName);
            if (path.StartsWith(configPath))
            {
                using var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var bs = new BufferedStream(fs);
                using var sr = new StreamReader(bs);

                return await sr.ReadToEndAsync();
            }
            
            return string.Empty;
        }

        /// <inheritdoc/>
        public async Task UpdateConfigurationFileAsync(Agent agent, string configurationContent, CancellationToken cancellationToken)
        {
            var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "configurations");
            var path = Path.Combine(configPath, agent.ConfigurationFileName);

            if (path.StartsWith(configPath))
            {
                await File.WriteAllTextAsync(path, configurationContent, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteConfigurationFileAsync(Agent agent, CancellationToken cancellationToken)
        {
            var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "configurations");
            var path = Path.Combine(configPath, agent.ConfigurationFileName);

            if (path.StartsWith(configPath))
            {
                File.Delete(path);

                agent.ConfigurationFileName = string.Empty;
                await UpdateAsync(agent, cancellationToken);
            }
        }
    }
}
