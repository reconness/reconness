using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog;
using ReconNess.Application.DataAccess;
using ReconNess.Application.Models;
using ReconNess.Application.Providers;
using ReconNess.Domain.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IScriptEngineProvider scriptEngineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="scriptEngineService"><see cref="IScriptEngineProvider"/></param>
        public AgentService(IUnitOfWork unitOfWork,
            IScriptEngineProvider scriptEngineService)
            : base(unitOfWork)
        {
            this.scriptEngineService = scriptEngineService;
        }

        /// <inheritdoc/>
        public async Task<List<Agent>> GetAgentsNoTrackingAsync(CancellationToken cancellationToken = default)
        {
            var result = GetAllQueryable()
                    .Select(agent => new Agent
                    {
                        Id = agent.Id,
                        Name = agent.Name,
                        LastRun = agent.LastRun,
                        Command = agent.Command,
                        AgentType = agent.AgentType,
                        CreatedBy = agent.CreatedBy,
                        PrimaryColor = agent.PrimaryColor,
                        SecondaryColor = agent.SecondaryColor,
                        Repository = agent.Repository,
                        AgentTrigger = agent.AgentTrigger,
                        Script = agent.Script,
                        Target = agent.Target,
                        Image = agent.Image,
                        ConfigurationFileName = agent.ConfigurationFileName,
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

        /// <inheritdoc/>
        public async Task<Agent> GetAgentNoTrackingAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await GetAllQueryableByCriteria(criteria)
                    .Select(agent => new Agent
                    {
                        Id = agent.Id,
                        Name = agent.Name,
                        Repository = agent.Repository,
                        Script = agent.Script,
                        Command = agent.Command,
                        AgentType = agent.AgentType,
                        AgentTrigger = agent.AgentTrigger,
                        ConfigurationFileName = agent.ConfigurationFileName,
                        Categories = agent.Categories.Select(category => new Category
                        {
                            Name = category.Name
                        })
                        .ToList()
                    })
                    .AsNoTracking()
                    .SingleOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Agent> GetAgentAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await GetAllQueryableByCriteria(criteria)
                    .Include(a => a.Categories)
                    .Include(a => a.AgentTrigger)
                    .Include(a => a.EventTracks)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Agent> GetAgentToRunAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await GetAllQueryableByCriteria(criteria)
                    .Include(a => a.Categories)
                    .Include(a => a.AgentTrigger)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<AgentMarketplace>> GetMarketplaceAsync(CancellationToken cancellationToken = default)
        {
            var client = new RestClient("https://raw.githubusercontent.com/");
            var request = new RestRequest("/reconness/reconness-agents/master/default-agents2.json");

            var response = await client.ExecuteGetAsync(request, cancellationToken);
            var agentMarketplaces = JsonConvert.DeserializeObject<AgentMarketplaces>(response.Content);

            return agentMarketplaces.Agents;
        }

        /// <inheritdoc/>
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
            else
            {
                _logger.Warn($"Invalid file path {path}");
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