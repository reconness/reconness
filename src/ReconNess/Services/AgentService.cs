using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Providers;
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
        private readonly IAuthProvider authProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="scriptEngineService"><see cref="IScriptEngineService"/></param>
        /// <param name="authProvider"><see cref="IAuthProvider"/></param>
        public AgentService(IUnitOfWork unitOfWork,
            IScriptEngineService scriptEngineService,
            IAuthProvider authProvider)
            : base(unitOfWork)
        {
            this.scriptEngineService = scriptEngineService;
            this.authProvider = authProvider;
        }

        /// <summary>
        /// <see cref="IAgentService.GetAgentsNoTrackingAsync(CancellationToken)"/>
        /// </summary>
        public async Task<List<Agent>> GetAgentsNoTrackingAsync(CancellationToken cancellationToken = default)
        {
            var result = this.GetAllQueryable(cancellationToken)
                    .Select(agent => new Agent
                    {
                        Id = agent.Id,
                        Name = agent.Name,
                        LastRun = agent.LastRun,
                        Command = agent.Command,
                        AgentType = agent.AgentType,
                        Categories = agent.Categories.Select(category => new Category
                        {
                            Name = category.Name
                        })
                        .ToList()
                    })
                    .AsNoTracking();

            return await result
                    .OrderBy(a => a.Categories.Single().Name)
                    .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="IAgentService.GetAgentNoTrackingAsync(Expression{Func{Agent, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<Agent> GetAgentNoTrackingAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                    .Select(agent => new Agent
                    {
                        Id = agent.Id,
                        Name = agent.Name,
                        Repository = agent.Repository,
                        Script = agent.Script,
                        Command = agent.Command,
                        AgentType = agent.AgentType,
                        AgentTrigger = agent.AgentTrigger,
                        Categories = agent.Categories.Select(category => new Category
                        {
                            Name = category.Name
                        })
                        .ToList()
                    })
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
        }

        /// <summary>
        /// <see cref="IAgentService.GetAgentAsync(Expression{Func{Agent, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<Agent> GetAgentAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default)
        {            
            return await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                    .Include(a => a.Categories)
                    .Include(a => a.AgentTrigger)
                    .Include(a => a.AgentHistories)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// <see cref="IAgentService.GetAgentToRunAsync(Expression{Func{Agent, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<Agent> GetAgentToRunAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                    .Include(a => a.Categories)
                    .Include(a => a.AgentTrigger)
                .SingleOrDefaultAsync();
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
            try
            {
                var client = new RestClient(scriptUrl);
                var request = new RestRequest();

                var response = await client.ExecuteGetAsync(request, cancellationToken);

                return response.Content;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// <see cref="IAgentService.DebugAsync(string, string, CancellationToken)"/>
        /// </summary>
        public async Task<ScriptOutput> DebugAsync(string script, string terminalOutput, CancellationToken cancellationToken = default)
        {
            return await this.scriptEngineService.TerminalOutputParseAsync(script, terminalOutput, 0);
        }

        /// <summary>
        /// <see cref="IAgentService.AddAgentHistoryAsync(Agent, CancellationToken)"/>
        /// </summary>
        public async Task<Agent> AddAgentAsync(Agent agent, string changeType, CancellationToken cancellationToken = default)
        {
            agent.AgentHistories = new List<AgentHistory>
            {
                new AgentHistory
                {
                    Username = this.authProvider.UserName(),
                    ChangeType = changeType
                }
            };

            return await this.AddAsync(agent);
        }

        /// <summary>
        /// <see cref="IAgentService.UpdateAgentAsync(Agent, CancellationToken)"/>
        /// </summary>
        public async Task UpdateAgentAsync(Agent agent, CancellationToken cancellationToken = default)
        {
            if (agent.AgentHistories == null)
            {
                agent.AgentHistories = new List<AgentHistory>();
            }

            agent.AgentHistories.Add(new AgentHistory
            {
                Username = this.authProvider.UserName(),
                ChangeType = "Agent Updated"
            });

            await this.UpdateAsync(agent);
        }
    }
}
