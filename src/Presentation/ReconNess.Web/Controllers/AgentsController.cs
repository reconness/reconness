using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private static readonly string ERROR_AGENT_EXIT = "An Agent with that name exist";
        private static readonly string ERROR_AGENT_MARKETPLACE_EXIT = "The agent is already installed";

        private readonly IMapper mapper;
        private readonly IAgentService agentService;
        private readonly IAgentRunnerService agentRunnerService;
        private readonly IAgentCategoryService categoryService;
        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;
        private readonly ISubdomainService subdomainService;
        private readonly IEventTrackService eventTrackService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="agentService"><see cref="IAgentService"/></param>
        /// <param name="agentRunnerService"><see cref="IAgentRunnerService"/></param>
        /// <param name="categoryService"><see cref="IAgentCategoryService"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        /// <param name="eventTrackService"><see cref="IEventTrackService"/></param>
        public AgentsController(
            IMapper mapper,
            IAgentService agentService,
            IAgentRunnerService agentRunnerService,
            IAgentCategoryService categoryService,
            ITargetService targetService,
            IRootDomainService rootDomainService,
            ISubdomainService subdomainService,
            IEventTrackService eventTrackService)
        {
            this.mapper = mapper;
            this.agentService = agentService;
            this.agentRunnerService = agentRunnerService;
            this.categoryService = categoryService;
            this.targetService = targetService;
            this.rootDomainService = rootDomainService;
            this.subdomainService = subdomainService;
            this.eventTrackService = eventTrackService;
        }

        /// <summary>
        /// Obtain the list of agents installed.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/agents
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of agents</returns>
        /// <response code="200">Returns the list of agents</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var agents = await agentService.GetAgentsNoTrackingAsync(cancellationToken);

            var agentsDto = mapper.Map<List<Agent>, List<AgentDto>>(agents);

            return Ok(agentsDto);
        }

        /// <summary>
        /// Obtain an agent.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/agents/{agentName}
        ///
        /// </remarks>
        /// <param name="agentName">The agent name</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The agent</returns>
        /// <response code="200">Returns the agent</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpGet("{agentName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] string agentName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(agentName))
            {
                return BadRequest();
            }

            var agent = await agentService.GetAgentNoTrackingAsync(t => t.Name == agentName, cancellationToken);
            if (agent == null)
            {
                return NotFound();
            }

            var agentDto = mapper.Map<Agent, AgentDto>(agent);
            if (!string.IsNullOrEmpty(agentDto.ConfigurationFileName))
            {
                agentDto.ConfigurationContent = await agentService.ReadConfigurationFileAsync(agentDto.ConfigurationFileName, cancellationToken);
                agentDto.ConfigurationPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "configurations");
            }

            return Ok(agentDto);
        }

        /// <summary>
        /// Obtain the marketplace of agents, with all the agents that we can install on reconness by default.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/agents/marketplace
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of agents in the marketplace</returns>
        /// <response code="200">Returns the list of agents in the marketplace</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("marketplace")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Marketplace(CancellationToken cancellationToken)
        {
            var marketplaces = await agentService.GetMarketplaceAsync(cancellationToken);

            var marketplaceDtos = mapper.Map<List<AgentMarketplace>, List<AgentMarketplaceDto>>(marketplaces);

            return Ok(marketplaceDtos);
        }

        /// <summary>
        /// Save a new agent.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/agents
        ///     {
        ///         "name": "mynewagent",
        ///         "command": "myagent -h -d {{rootdomain}}",
        ///         "repository": "www.github.com/myaccount/myproject",
        ///         "agentType": "subdomain",
        ///         "categories": "scan subdomains"
        ///     }
        ///
        /// </remarks>
        /// <param name="agentDto">The agent dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="200">Return the created agent</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] AgentDto agentDto, CancellationToken cancellationToken)
        {
            var agentExist = await agentService.AnyAsync(t => t.Name == agentDto.Name, cancellationToken);
            if (agentExist)
            {
                return BadRequest(ERROR_AGENT_EXIT);
            }

            var agent = mapper.Map<AgentDto, Agent>(agentDto);
            if (string.IsNullOrEmpty(agent.Script))
            {
                agent.Script = "return new ReconNess.Core.Models.ScriptOutput();";
            }

            var insertedAgent = await this.agentService.AddAsync(agent, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Agent = insertedAgent,
                Description = $"Agent {agent.Name} Added"
            }, cancellationToken);

            var insertedAgentDto = this.mapper.Map<Agent, AgentDto>(insertedAgent);

            return Ok(insertedAgentDto);
        }

        /// <summary>
        /// Update an agent.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/agents/{id}
        ///     { 
        ///         "name": "mynewagent",
        ///         "command": "myagent -h -d {{rootdomain}}",
        ///         "repository": "www.github.com/myaccount/myproject",
        ///         "agentType": "subdomain",
        ///         "categories": "scan subdomains",
        ///         "script": "// the script here"
        ///     }
        ///
        /// </remarks>
        /// <param name="id">The agent id</param>
        /// <param name="agentDto">The agent dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] AgentDto agentDto, CancellationToken cancellationToken)
        {
            var agent = await agentService.GetAgentAsync(t => t.Id == id, cancellationToken);
            if (agent == null)
            {
                return NotFound();
            }

            var agentExist = agent.Name != agentDto.Name && await agentService.AnyAsync(t => t.Name == agentDto.Name, cancellationToken);
            if (agentExist)
            {
                return BadRequest(ERROR_AGENT_EXIT);
            }

            agent.Name = agentDto.Name;
            agent.Repository = agentDto.Repository;
            agent.Command = agentDto.Command;
            agent.Script = agentDto.Script;
            agent.AgentType = agentDto.AgentType;
            agent.PrimaryColor = agentDto.PrimaryColor;
            agent.SecondaryColor = agentDto.SecondaryColor;
            agent.Target = agentDto.Target;
            agent.Image = agentDto.Image;
            agent.Categories = await this.categoryService.GetCategoriesAsync(agent.Categories, agentDto.Categories, cancellationToken);

            if (agent.AgentTrigger == null)
            {
                agent.AgentTrigger = new AgentTrigger();
            }

            agent.AgentTrigger.SkipIfRunBefore = agentDto.TriggerSkipIfRunBefore;
            agent.AgentTrigger.TargetHasBounty = agentDto.TriggerTargetHasBounty;
            agent.AgentTrigger.TargetIncExcName = agentDto.TriggerTargetIncExcName;
            agent.AgentTrigger.TargetName = agentDto.TriggerTargetName;
            agent.AgentTrigger.RootdomainHasBounty = agentDto.TriggerRootdomainHasBounty;
            agent.AgentTrigger.RootdomainIncExcName = agentDto.TriggerRootdomainIncExcName;
            agent.AgentTrigger.RootdomainName = agentDto.TriggerRootdomainName;
            agent.AgentTrigger.SubdomainHasBounty = agentDto.TriggerSubdomainHasBounty;
            agent.AgentTrigger.SubdomainIsAlive = agentDto.TriggerSubdomainIsAlive;
            agent.AgentTrigger.SubdomainIsMainPortal = agentDto.TriggerSubdomainIsMainPortal;
            agent.AgentTrigger.SubdomainHasHttpOrHttpsOpen = agentDto.TriggerSubdomainHasHttpOrHttpsOpen;
            agent.AgentTrigger.SubdomainIncExcName = agentDto.TriggerSubdomainIncExcName;
            agent.AgentTrigger.SubdomainName = agentDto.TriggerSubdomainName;
            agent.AgentTrigger.SubdomainIncExcServicePort = agentDto.TriggerSubdomainIncExcServicePort;
            agent.AgentTrigger.SubdomainServicePort = agentDto.TriggerSubdomainServicePort;
            agent.AgentTrigger.SubdomainIncExcIP = agentDto.TriggerSubdomainIncExcIP;
            agent.AgentTrigger.SubdomainIP = agentDto.TriggerSubdomainIP;
            agent.AgentTrigger.SubdomainIncExcTechnology = agentDto.TriggerSubdomainIncExcTechnology;
            agent.AgentTrigger.SubdomainTechnology = agentDto.TriggerSubdomainTechnology;
            agent.AgentTrigger.SubdomainIncExcLabel = agentDto.TriggerSubdomainIncExcLabel;
            agent.AgentTrigger.SubdomainLabel = agentDto.TriggerSubdomainLabel;

            await this.agentService.UpdateAsync(agent, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Agent = agent,
                Description = $"Agent {agent.Name} Updated"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete an agent.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/agents/{agentName}
        ///
        /// </remarks>
        /// <param name="agentName">The agent name</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpDelete("{agentName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete([FromRoute] string agentName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(agentName))
            {
                return BadRequest();
            }

            var agent = await agentService.GetByCriteriaAsync(t => t.Name == agentName, cancellationToken);
            if (agent == null)
            {
                return NotFound();
            }

            await agentService.DeleteAsync(agent, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Description = $"Agent {agent.Name} Deleted"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete multiple agents.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/agents/batch
        ///     ["agente1", "agente2", agente3]
        ///
        /// </remarks>
        /// <param name="agentNames">The agent name list</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpDelete("batch")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteBatch([FromBody] IList<string> agentNames, CancellationToken cancellationToken)
        {
            if (agentNames == null || agentNames.Count == 0)
            {
                return BadRequest();
            }

            var agentsEntities = new List<Agent>();
            foreach (var agentName in agentNames)
            {
                var agent = await this.agentService.GetByCriteriaAsync(a => a.Name == agentName, cancellationToken);
                if (agent != null)
                {
                    agentsEntities.Add(agent);
                    
                }
            }

            await this.agentService.DeleteRangeAsync(agentsEntities, cancellationToken);

            foreach (var agent in agentsEntities)
            {
                await this.eventTrackService.AddAsync(new EventTrack
                {
                    Description = $"Agent {agent.Name} Deleted"
                }, cancellationToken);
            }

            return NoContent();
        }

        /// <summary>
        /// Install a new agent from the marketplace.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/agents/install
        ///
        /// </remarks>
        /// <param name="agentDefaultDto">The agent dto from the marketplace</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The agent installed</returns>
        /// <response code="200">Returns the agent installed</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("install")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Install([FromBody] AgentMarketplaceDto agentDefaultDto, CancellationToken cancellationToken)
        {
            var agentExist = await this.agentService.AnyAsync(t => t.Id == agentDefaultDto.Id, cancellationToken);
            if (agentExist)
            {
                return BadRequest(ERROR_AGENT_MARKETPLACE_EXIT);
            }

            var agent = mapper.Map<AgentMarketplaceDto, Agent>(agentDefaultDto);

            agent.Script = await agentService.GetScriptAsync(agentDefaultDto.ScriptUrl, cancellationToken);

            var agentInstalled = await this.agentService.AddAsync(agent, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Agent = agentInstalled,
                Description = $"Agent {agentInstalled.Name} Installed"
            }, cancellationToken);

            return Ok(this.mapper.Map<Agent, AgentDto>(agentInstalled));
        }

        /// <summary>
        /// Debug an agent.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/agents/debug
        ///     {
        ///         "terminalOutput": "the terminal output that we want to debug",
        ///         "script": "// the script that is going to parse and debug the terminal output define above"
        ///     }
        ///
        /// </remarks>
        /// <param name="agentDebugDto">The agent debug dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The debug output</returns>
        /// <response code="200">Returns thedebug output</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpPost("debug")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Debug([FromBody] AgentDebugDto agentDebugDto, CancellationToken cancellationToken)
        {
            try
            {
                var scriptOutput = await agentService.DebugAsync(agentDebugDto.Script, agentDebugDto.TerminalOutput, cancellationToken);

                return Ok(scriptOutput);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        /// <summary>
        /// Run an agent.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/agents/run
        ///     {
        ///         "agent": "myagent",
        ///         "command": "myagent -h -d {{subdomain}}",
        ///         "target": "mytarget",
        ///         "rootdomain": "myrootdomain.com"
        ///         "subdomain": "www.mysubdomain.com",
        ///         "activateNotification": true
        ///     }
        ///
        /// </remarks>
        /// <param name="agentRunnerDto">The agent dto to run</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="200">The channel</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("run")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RunAgent([FromBody] AgentRunnerDto agentRunnerDto, CancellationToken cancellationToken)
        {
            var agent = await agentService.GetAgentToRunAsync(a => a.Name == agentRunnerDto.Agent, cancellationToken);
            if (agent == null)
            {
                return BadRequest();
            }

            Target target = default;
            if (!string.IsNullOrWhiteSpace(agentRunnerDto.Target))
            {
                target = await targetService.GetTargetNotTrackingAsync(t => t.Name == agentRunnerDto.Target, cancellationToken);
                if (target == null)
                {
                    return BadRequest();
                }
            }

            RootDomain rootDomain = default;
            if (!string.IsNullOrWhiteSpace(agentRunnerDto.RootDomain))
            {
                rootDomain = await rootDomainService.GetRootDomainNoTrackingAsync(t => t.Target == target && t.Name == agentRunnerDto.RootDomain, cancellationToken);
                if (rootDomain == null)
                {
                    return NotFound();
                }
            }

            Subdomain subdomain = default;
            if (rootDomain != null && !string.IsNullOrWhiteSpace(agentRunnerDto.Subdomain))
            {
                subdomain = await subdomainService.GetSubdomainAsync(s => s.RootDomain == rootDomain && s.Name == agentRunnerDto.Subdomain, cancellationToken);
                if (subdomain == null)
                {
                    return NotFound();
                }
            }

            var channel = await agentRunnerService.RunAgentAsync(
                new AgentRunnerInfo
                {
                    Agent = agent,
                    Target = target,
                    RootDomain = rootDomain,
                    Subdomain = subdomain,
                    ActivateNotification = agentRunnerDto.ActivateNotification,
                    Command = agentRunnerDto.Command
                }, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Agent = agent,
                Description = $"Agent {agent.Name} Running"
            }, cancellationToken);

            return Ok(channel);
        }

        /// <summary>
        /// Stop an agent.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/agents/stop
        ///     {
        ///         "channel": "the_channel"
        ///     }
        ///
        /// </remarks>
        /// <param name="channel">The channel</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("stop")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> StopAgent([FromBody] string channel, CancellationToken cancellationToken)
        {
            await agentRunnerService.StopAgentAsync(channel, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Obtain the list of running agents.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/agents/running
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of running agents</returns>
        /// <response code="200">Returns the list of running agents</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("running")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RunningAgent(CancellationToken cancellationToken)
        {
            var agentsRunning = await agentRunnerService.RunningAgentsAsync(cancellationToken);

            return Ok(agentsRunning);
        }

        /// <summary>
        /// Obtain the terminal output
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/agents/terminal/{channel}
        ///
        /// </remarks>
        /// <param name="channel">The channel</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="200">Terminal Output</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpGet("terminal/{channel}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> TerminalAgent(string channel, CancellationToken cancellationToken)
        {
            var result = await agentRunnerService.TerminalOutputAsync(channel, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Upload the configuration file, that is going to be save inside the folder
        /// /app/Content/configurations.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/agents/upload/{id}
        ///
        /// </remarks>
        /// <param name="agentName">The agent name</param>
        /// <param name="file">The file with the agent configuration</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("upload/{agentName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadConfiguration([FromRoute] string agentName, IFormFile file, CancellationToken cancellationToken)
        {
            if (file.Length == 0 || string.IsNullOrEmpty(agentName))
            {
                return BadRequest();
            }

            var agent = await agentService.GetByCriteriaAsync(t => t.Name == agentName, cancellationToken);
            if (agent == null)
            {
                return NotFound();
            }

            var configurationFileName = string.Empty;
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                configurationFileName = file.FileName.Replace(c.ToString(), "");
            }

            var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "configurations");
            var fileNamePath = Path.Combine(configPath, configurationFileName);
            if (!fileNamePath.StartsWith(configPath))
            {
                return BadRequest("The path to save the file is invalid.");
            }

            if (System.IO.File.Exists(fileNamePath))
            {
                return BadRequest("We have a file with that name inside the configuration folder, please change the filename and try again.");
            }

            using (var stream = new FileStream(fileNamePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            if (!string.IsNullOrEmpty(agent.ConfigurationFileName))
            {
                var removeFile = Path.Combine(configPath, agent.ConfigurationFileName);
                if (removeFile.StartsWith(configPath))
                {
                    System.IO.File.Delete(removeFile);
                }
            }

            agent.ConfigurationFileName = file.FileName;
            await agentService.UpdateAsync(agent, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Agent = agent,
                Description = $"Agent {agent.Name} configuration uploaded"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// save an agent configuration file.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/agents/configuration/{agentName}
        ///
        /// </remarks>
        /// <param name="agentName">The agent name</param>
        /// <param name="agentDto">The agent dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpPut("configuration/{agentName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateConfiguration([FromRoute] string agentName, [FromBody] AgentDto agentDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(agentName))
            {
                return BadRequest();
            }

            var agent = await agentService.GetByCriteriaAsync(t => t.Name == agentName, cancellationToken);
            if (agent == null)
            {
                return NotFound();
            }

            if (!agent.Name.Equals(agentDto.Name))
            {
                return BadRequest();
            }

            await agentService.UpdateConfigurationFileAsync(agent, agentDto.ConfigurationContent, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Agent = agent,
                Description = $"Agent {agent.Name} configuration updated"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete an agent configuration file.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/agents/configuration/{agentName}
        ///
        /// </remarks>
        /// <param name="agentName">The agent name</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpDelete("configuration/{agentName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteConfiguration([FromRoute] string agentName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(agentName))
            {
                return BadRequest();
            }

            var agent = await agentService.GetByCriteriaAsync(t => t.Name == agentName, cancellationToken);
            if (agent == null)
            {
                return NotFound();
            }

            await agentService.DeleteConfigurationFileAsync(agent, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Agent = agent,
                Description = $"Agent {agent.Name} configuration deleted"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Obtain the configuratin path of the agents.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/agents/configuration/path
        ///
        /// </remarks>
        /// <returns>The configuration path</returns>
        /// <response code="200">Returns the configuration path</response>
        [HttpGet("configuration/path")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ConfigurationPath()
        {
            var configurationPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "configurations");
            return Ok(configurationPath);
        }
    }
}
