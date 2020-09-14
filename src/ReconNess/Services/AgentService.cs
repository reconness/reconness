using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentService"/>
    /// </summary>
    public class AgentService : Service<Agent>, IService<Agent>, IAgentService
    {
        private readonly IScriptEngineService scriptEngineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="scriptEngineService"><see cref="IScriptEngineService"/></param>
        public AgentService(IUnitOfWork unitOfWork,
            IScriptEngineService scriptEngineService)
            : base(unitOfWork)
        {
            this.scriptEngineService = scriptEngineService;
        }

        /// <summary>
        /// <see cref="IAgentService.GetAllAgentsAsync(CancellationToken)"/>
        /// </summary>
        public async Task<List<Agent>> GetAllAgentsAsync(CancellationToken cancellationToken = default)
        {
            var result = await this.GetAllQueryable(cancellationToken)
                .Include(a => a.AgentCategories)
                    .ThenInclude(c => c.Category)
                .Include(a => a.AgentTypes)
                    .ThenInclude(c => c.Type)
                .ToListAsync();

            return result.OrderBy(a => a.AgentCategories.FirstOrDefault()?.Category?.Name).ToList();
        }

        /// <summary>
        /// <see cref="IAgentService.GetAgentAsync(Expression{Func{Agent, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<Agent> GetAgentAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                .Include(a => a.AgentCategories)
                    .ThenInclude(c => c.Category)
                .Include(a => a.AgentTypes)
                    .ThenInclude(c => c.Type)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// <see cref="IAgentService.GetMarketplaceAsync(CancellationToken)"/>
        /// </summary>
        public async Task<List<AgentMarketplace>> GetMarketplaceAsync(CancellationToken cancellationToken = default)
        {
            var client = new RestClient("https://raw.githubusercontent.com/");
            var request = new RestRequest("/reconness/reconness-agents/master/default-agents.json");

            var response = await client.ExecuteGetAsync(request, cancellationToken);
            var agentMarketplaces = JsonConvert.DeserializeObject<AgentMarketplaces>(response.Content);

            return agentMarketplaces.Agents;
        }

        /// <summary>
        /// <see cref="IAgentService.GetAgentScript(string, CancellationToken)"/>
        /// </summary>
        public async Task<string> GetAgentScript(string scriptUrl, CancellationToken cancellationToken)
        {
            var client = new RestClient(scriptUrl);
            var request = new RestRequest();

            var response = await client.ExecuteGetAsync(request, cancellationToken);

            return response.Content;
        }

        /// <summary>
        /// <see cref="IAgentService.DebugAsync(string, string, CancellationToken)"/>
        /// </summary>
        public async Task<ScriptOutput> DebugAsync(string script, string terminalOutput, CancellationToken cancellationToken = default)
        {
            return await this.scriptEngineService.TerminalOutputParseAsync(script, terminalOutput, 0);
        }

        /// <summary>
        /// <see cref="IAgentService.IsBySubdomainAsync(string, CancellationToken)"/>
        /// </summary>
        public async Task<bool> IsBySubdomainAsync(string agentName, CancellationToken cancellationToken)
        {
            var agent = await this.GetAgentAsync(a => a.Name == agentName, cancellationToken);

            return agent != null && agent.AgentTypes.Any(t => t.Type.Name == AgentTypes.SUBDOMAIN);
        }
    }
}
