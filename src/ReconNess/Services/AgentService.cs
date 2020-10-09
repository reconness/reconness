using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog;
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
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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
        /// <see cref="IAgentService.GetAllWithIncludeAsync(CancellationToken)"/>
        /// </summary>
        public async Task<List<Agent>> GetAllWithIncludeAsync(CancellationToken cancellationToken = default)
        {
            var result = await this.GetAllQueryable(cancellationToken)
                .Include(a => a.AgentCategories)
                    .ThenInclude(c => c.Category)
                .ToListAsync();

            return result.OrderBy(a => a.AgentCategories.FirstOrDefault()?.Category?.Name).ToList();
        }

        /// <summary>
        /// <see cref="IAgentService.GetWithIncludeAsync(Expression{Func{Agent, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<Agent> GetWithIncludeAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                .Include(a => a.AgentCategories)
                    .ThenInclude(c => c.Category)
                .Include(a => a.AgentTrigger)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// <see cref="IAgentService.GetMarketplaceAsync(CancellationToken)"/>
        /// </summary>
        public async Task<List<AgentMarketplace>> GetMarketplaceAsync(CancellationToken cancellationToken = default)
        {
            var client = new RestClient("https://raw.githubusercontent.com/");
            var request = new RestRequest("/reconness/reconness-agents/master/default-agents1.5.json");

            var response = await client.ExecuteGetAsync(request, cancellationToken);
            var agentMarketplaces = JsonConvert.DeserializeObject<AgentMarketplaces>(response.Content);

            return agentMarketplaces.Agents;
        }

        /// <summary>
        /// <see cref="IAgentService.GetScriptAsync(string, CancellationToken)"/>
        /// </summary>
        public async Task<string> GetScriptAsync(string scriptUrl, CancellationToken cancellationToken)
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
    }
}
