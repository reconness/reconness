using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAgentService agentService;
        private readonly IAgentRunnerService agentRunnerService;
        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;
        private readonly ICategoryService categoryService;
        private readonly ISubdomainService subdomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="agentService"><see cref="IAgentService"/></param>
        /// <param name="agentRunnerService"><see cref="IAgentRunnerService"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="categoryService"><see cref="ICategoryService"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        public AgentsController(
            IMapper mapper,
            IAgentService agentService,
            IAgentRunnerService agentRunnerService,
            ITargetService targetService,
            IRootDomainService rootDomainService,
            ICategoryService categoryService,
            ISubdomainService subdomainService)
        {
            this.mapper = mapper;
            this.agentService = agentService;
            this.agentRunnerService = agentRunnerService;
            this.targetService = targetService;
            this.rootDomainService = rootDomainService;
            this.categoryService = categoryService;
            this.subdomainService = subdomainService;
        }

        // GET api/agents
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var agents = await this.agentService.GetAllAgentsWithCategoryAsync(cancellationToken);

            var agentsDto = this.mapper.Map<List<Agent>, List<AgentDto>>(agents);
            return Ok(agentsDto);
        }

        // GET api/agents/{agentName}
        [HttpGet("{agentName}")]
        public async Task<IActionResult> Get(string agentName, CancellationToken cancellationToken)
        {
            var agent = await this.agentService.GetAgentWithCategoryAsync(t => t.Name == agentName, cancellationToken);
            if (agent == null)
            {
                return NotFound();
            }

            return Ok(this.mapper.Map<Agent, AgentDto>(agent));
        }

        // GET api/agents/defaultToInstall
        [HttpGet("defaultToInstall")]
        public async Task<IActionResult> GetDefaultToInstall(CancellationToken cancellationToken)
        {
            var agentDefaults = await this.agentService.GetDefaultAgentsToInstallAsync(cancellationToken);

            var agentsDto = this.mapper.Map<List<AgentDefault>, List<AgentDefaultDto>>(agentDefaults);

            return Ok(agentsDto);
        }

        // POST api/agents
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AgentDto agentDto, CancellationToken cancellationToken)
        {
            if (await this.agentService.AnyAsync(t => t.Name == agentDto.Name))
            {
                return BadRequest("There is an Agent with that name in the DB");
            }

            var agent = this.mapper.Map<AgentDto, Agent>(agentDto);
            agent.Script = "return new ReconNess.Core.Models.ScriptOutput();";

            await this.agentService.AddAsync(agent, cancellationToken);

            return NoContent();
        }

        // PUT api/agents/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] AgentDto agentDto, CancellationToken cancellationToken)
        {
            var agent = await this.agentService.GetAgentWithCategoryAsync(t => t.Id == id, cancellationToken);
            if (agent == null)
            {
                return NotFound();
            }

            if (agent.Name != agentDto.Name && await this.rootDomainService.AnyAsync(t => t.Name == agentDto.Name))
            {
                return BadRequest("There is an Agent with that name in the DB");
            }

            agent.Name = agentDto.Name;
            agent.Repository = agentDto.Repository;
            agent.AgentCategories = await this.categoryService.GetCategoriesAsync(agent.AgentCategories, agentDto.Categories, cancellationToken);
            agent.Command = agentDto.Command;
            agent.IsBySubdomain = agentDto.IsBySubdomain;
            agent.OnlyIfIsAlive = agentDto.OnlyIfIsAlive;
            agent.OnlyIfHasHttpOpen = agentDto.OnlyIfHasHttpOpen;
            agent.SkipIfRanBefore = agentDto.SkipIfRanBefore;
            agent.NotifyIfAgentDone = agentDto.NotifyIfAgentDone;
            agent.NotifyNewFound = agentDto.NotifyNewFound;

            if (agent.AgentNotification == null)
            {
                agent.AgentNotification = new AgentNotification();
            }

            agent.AgentNotification.SubdomainPayload = agentDto.SubdomainPayload;
            agent.AgentNotification.IpAddressPayload = agentDto.IpAddressPayload;
            agent.AgentNotification.IsAlivePayload = agentDto.IsAlivePayload;
            agent.AgentNotification.HasHttpOpenPayload = agentDto.HasHttpOpenPayload;
            agent.AgentNotification.TakeoverPayload = agentDto.TakeoverPayload;
            agent.AgentNotification.DirectoryPayload = agentDto.DirectoryPayload;
            agent.AgentNotification.ServicePayload = agentDto.ServicePayload;
            agent.AgentNotification.NotePayload = agentDto.NotePayload;

            agent.Script = agentDto.Script;

            await this.agentService.UpdateAsync(agent, cancellationToken);

            return NoContent();
        }

        // DELETE api/agents/{agentName}
        [HttpDelete("{agentName}")]
        public async Task<IActionResult> Delete(string agentName, CancellationToken cancellationToken)
        {
            var agent = await this.agentService.GetByCriteriaAsync(t => t.Name == agentName, cancellationToken);
            if (agent == null)
            {
                return NotFound();
            }

            await this.agentService.DeleteAsync(agent, cancellationToken);

            return NoContent();
        }

        // POST api/agents/install
        [HttpPost("install")]
        public async Task<IActionResult> Install([FromBody] AgentDefaultDto agentDefaultDto, CancellationToken cancellationToken)
        {
            if (await this.agentService.AnyAsync(t => t.Name == agentDefaultDto.Name))
            {
                return BadRequest("There is an Agent with that name in the DB");
            }

            var agent = this.mapper.Map<AgentDefaultDto, Agent>(agentDefaultDto);

            agent.Script = await this.agentService.GetAgentScript(agentDefaultDto.ScriptUrl, cancellationToken);

            var agentInstalled = await this.agentService.AddAsync(agent, cancellationToken);

            return Ok(this.mapper.Map<Agent, AgentDto>(agentInstalled));
        }

        // POST api/agents/run
        [HttpPost("run")]
        public async Task<IActionResult> RunAgent([FromBody] AgentRunnerDto agentRunnerDto, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == agentRunnerDto.Target, cancellationToken);
            if (target == null)
            {
                return BadRequest();
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(t => t.Name == agentRunnerDto.RootDomain && t.Target == target, cancellationToken);
            if (rootDomain == null)
            {
                return BadRequest();
            }

            Subdomain subdomain = null;
            if (!string.IsNullOrWhiteSpace(agentRunnerDto.Subdomain))
            {
                subdomain = (await this.subdomainService.GetSubdomainsAsync(rootDomain, agentRunnerDto.Subdomain, cancellationToken)).FirstOrDefault();
                if (subdomain == null)
                {
                    return NotFound();
                }
            }

            var agent = await agentService.GetAgentWithCategoryAsync(a => a.Name == agentRunnerDto.Agent, cancellationToken);
            if (agent == null)
            {
                return BadRequest();
            }

            await this.agentRunnerService.RunAsync(
                new AgentRunner
                {
                    Agent = agent,
                    Target = target,
                    RootDomain = rootDomain,
                    Subdomain = subdomain,
                    ActivateNotification = agentRunnerDto.ActivateNotification,
                    Command = agentRunnerDto.Command
                }, cancellationToken);

            return NoContent();
        }

        // POST api/agents/stop
        [HttpPost("stop")]
        public async Task<ActionResult> StopAgent([FromBody] AgentRunnerDto agentRunnerDto, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == agentRunnerDto.Target, cancellationToken);
            if (target == null)
            {
                return BadRequest();
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(t => t.Name == agentRunnerDto.RootDomain && t.Target == target, cancellationToken);
            if (rootDomain == null)
            {
                return BadRequest();
            }

            Subdomain subdomain = null;
            if (!string.IsNullOrWhiteSpace(agentRunnerDto.Subdomain))
            {
                subdomain = await this.subdomainService.GetByCriteriaAsync(s => s.RootDomain == rootDomain && s.Name == agentRunnerDto.Subdomain, cancellationToken);
                if (subdomain == null)
                {
                    return NotFound();
                }
            }

            var agent = await agentService.GetByCriteriaAsync(a => a.Name == agentRunnerDto.Agent, cancellationToken);
            if (agent == null)
            {
                return BadRequest();
            }

            var task = this.agentRunnerService.StopAsync(new AgentRunner
            {
                Agent = agent,
                Target = target,
                RootDomain = rootDomain,
                Subdomain = subdomain
            }, false, cancellationToken);

            return NoContent();
        }

        // GET api/agents/running/{targetName}/{rootDomainName}/{subdomainName}
        [HttpGet("running/{targetName}/{rootDomainName}/{subdomainName}")]
        public async Task<ActionResult> RunningAgent(string targetName, string rootDomainName, string subdomainName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return BadRequest();
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(t => t.Name == rootDomainName && t.Target == target, cancellationToken);
            if (rootDomain == null)
            {
                return BadRequest();
            }

            Subdomain subdomain = null;
            if (!string.IsNullOrWhiteSpace(subdomainName) && !"undefined".Equals(subdomainName))
            {
                subdomain = await this.subdomainService.GetByCriteriaAsync(s => s.RootDomain == rootDomain && s.Name == subdomainName, cancellationToken);
                if (subdomain == null)
                {
                    return NotFound();
                }
            }

            var agentsRunning = await this.agentRunnerService.RunningAsync(new AgentRunner
            {
                Target = target,
                RootDomain = rootDomain,
                Subdomain = subdomain
            }, cancellationToken);

            return Ok(agentsRunning);
        }

        // POST api/agents/debug
        [HttpPost("debug")]
        public async Task<ActionResult> Debug([FromBody] AgentDebugDto agentDebugDto, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await this.agentService.DebugAsync(agentDebugDto.Script, agentDebugDto.TerminalOutput, cancellationToken));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
