using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAgentService agentService;
        private readonly ITargetService targetService;
        private readonly ICategoryService categoryService;
        private readonly ISubdomainService subdomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="agentService"><see cref="IAgentService"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="categoryService"><see cref="ICategoryService"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        public AgentsController(
            IMapper mapper,
            IAgentService agentService,
            ITargetService targetService,
            ICategoryService categoryService,
            ISubdomainService subdomainService)
        {
            this.mapper = mapper;
            this.agentService = agentService;
            this.targetService = targetService;
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
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

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
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var agent = await this.agentService.GetAgentWithCategoryAsync(t => t.Id == id, cancellationToken);
            if (agent == null)
            {
                return NotFound();
            }

            if (agent.Name != agentDto.Name && await this.targetService.AnyAsync(t => t.Name == agentDto.Name))
            {
                return BadRequest("There is an Agent with that name in the DB");
            }

            agent.Name = agentDto.Name;
            agent.AgentCategories = await this.categoryService.GetCategoriesAsync(agent.AgentCategories, agentDto.Categories, cancellationToken);
            agent.Command = agentDto.Command;
            agent.IsBySubdomain = agentDto.IsBySubdomain;
            agent.OnlyIfIsAlive = agentDto.OnlyIfIsAlive;
            agent.OnlyIfHasHttpOpen = agentDto.OnlyIfHasHttpOpen;
            agent.SkipIfRanBefore = agentDto.SkipIfRanBefore;
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
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

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
        public async Task<IActionResult> RunAgent([FromBody] AgentRunDto agentRunDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetAllQueryableByCriteria(t => t.Name == agentRunDto.Target, cancellationToken)
                .Include(t => t.Subdomains)
                    .ThenInclude(s => s.ServiceHttp)
                .Include(t => t.Subdomains)
                    .ThenInclude(s => s.Services)
                .FirstOrDefaultAsync(cancellationToken);

            if (target == null)
            {
                return BadRequest();
            }

            Subdomain subdomain = null;
            if (!string.IsNullOrWhiteSpace(agentRunDto.Subdomain))
            {
                subdomain = await this.subdomainService.GetAllQueryableByCriteria(s => s.Target == target && s.Name == agentRunDto.Subdomain, cancellationToken)
                    .Include(s => s.Services)
                    .Include(s => s.ServiceHttp)
                        .ThenInclude(s => s.Directories)
                    .Include(s => s.Labels)
                        .ThenInclude(l => l.Label)
                    .FirstOrDefaultAsync(cancellationToken);

                if (subdomain == null)
                {
                    return NotFound();
                }
            }

            var agent = await agentService.GetByCriteriaAsync(a => a.Name == agentRunDto.Agent, cancellationToken);
            if (agent == null)
            {
                return BadRequest();
            }

            await this.agentService.RunAsync(target, subdomain, agent, agentRunDto.Command, cancellationToken);

            return NoContent();
        }

        // POST api/agents/stop
        [HttpPost("stop")]
        public async Task<ActionResult> StopAgent([FromBody] AgentRunDto agentRunDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == agentRunDto.Target, cancellationToken);
            if (target == null)
            {
                return BadRequest();
            }

            Subdomain subdomain = null;
            if (!string.IsNullOrWhiteSpace(agentRunDto.Subdomain))
            {
                subdomain = await this.subdomainService.GetByCriteriaAsync(s => s.Target == target && s.Name == agentRunDto.Subdomain, cancellationToken);
                if (subdomain == null)
                {
                    return NotFound();
                }
            }

            var agent = await agentService.GetByCriteriaAsync(a => a.Name == agentRunDto.Agent, cancellationToken);
            if (agent == null)
            {
                return BadRequest();
            }

            var task = this.agentService.StopAsync(target, subdomain, agent, cancellationToken);

            return NoContent();
        }

        // POST api/agents/debug
        [HttpPost("debug")]
        public async Task<ActionResult> Debug([FromBody] AgentDebugDto agentDebugDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                return Ok(await this.agentService.DebugAsync(agentDebugDto.TerminalOutput, agentDebugDto.Script, cancellationToken));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
