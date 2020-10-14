using Microsoft.EntityFrameworkCore;
using NLog;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="ITargetService"/>
    /// </summary>
    public class TargetService : Service<Target>, IService<Target>, ITargetService, ISaveTerminalOutputParseService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IRootDomainService rootDomainService;
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ITargetService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="notificationService"><see cref="INotificationService"/></param>
        public TargetService(IUnitOfWork unitOfWork,
            IRootDomainService rootDomainService,
            INotificationService notificationService)
            : base(unitOfWork)
        {
            this.rootDomainService = rootDomainService;
            this.notificationService = notificationService;
        }

        /// <summary>
        /// <see cref="ITargetService.GetAllWithIncludeAsync(Expression{Func{Target, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<List<Target>> GetAllWithIncludeAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken)
        {
            return await this.GetAllQueryableByCriteria(predicate, cancellationToken)
                        .Include(a => a.RootDomains)
                        .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="ITargetService.GetWithIncludeAsync(Expression{Func{Target, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<Target> GetWithIncludeAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken)
        {
            return await this.GetAllQueryableByCriteria(predicate, cancellationToken)
                       .Include(a => a.RootDomains)
                           .ThenInclude(a => a.Subdomains)
                               .ThenInclude(s => s.Services)
                       .Include(a => a.RootDomains)
                           .ThenInclude(a => a.Subdomains)
                               .ThenInclude(s => s.Notes)
                       .Include(a => a.RootDomains)
                           .ThenInclude(a => a.Notes)
                       .FirstOrDefaultAsync(cancellationToken);
        }

        /// <see cref="ITargetService.DeleteTargetAsync(Target, CancellationToken)"/>
        /// </summary>
        public async Task DeleteTargetAsync(Target target, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.UnitOfWork.BeginTransaction(cancellationToken);

                this.rootDomainService.DeleteRootDomains(target.RootDomains, cancellationToken);

                await this.DeleteAsync(target, cancellationToken);
            }
            catch (Exception ex)
            {
                this.UnitOfWork.Rollback(cancellationToken);
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="ISaveTerminalOutputParseService.UpdateAgentRanAsync(AgentRunner, CancellationToken)"/>
        /// </summary>
        public async Task UpdateAgentRanAsync(AgentRunner agentRunner, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var target = agentRunner.Target;
            var agentName = agentRunner.Agent.Name;

            if (string.IsNullOrWhiteSpace(target.AgentsRanBefore))
            {
                target.AgentsRanBefore = agentName;
                await this.UpdateAsync(target, cancellationToken);
            }
            else if (!target.AgentsRanBefore.Contains(agentName))
            {
                target.AgentsRanBefore = string.Join(", ", target.AgentsRanBefore, agentName);
                await this.UpdateAsync(target, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="ISaveTerminalOutputParseService.SaveTerminalOutputParseAsync(AgentRunner, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveTerminalOutputParseAsync(AgentRunner agentRunner, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await this.NeedAddNewRootDomain(agentRunner, terminalOutputParse.RootDomain, cancellationToken))
            {
                agentRunner.RootDomain = await this.AddTargetNewRootDomainAsync(agentRunner.Target, terminalOutputParse.RootDomain, cancellationToken);
                if (agentRunner.ActivateNotification)
                {
                    await this.notificationService.SendAsync(NotificationType.SUBDOMAIN, new[]
                    {
                        ("{{rootDomain}}", agentRunner.RootDomain.Name)
                    }, cancellationToken);
                }
            }

            if (agentRunner.RootDomain != null)
            {
                await this.rootDomainService.SaveTerminalOutputParseAsync(agentRunner, terminalOutputParse, cancellationToken);
            }
        }

        /// <summary>
        /// If we need to add a new RootDomain
        /// </summary>
        /// <param name="agentRunner">The Agent running</param>
        /// <param name="rootDomain">The root domain</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>If we need to add a new RootDomain</returns>
        private async Task<bool> NeedAddNewRootDomain(AgentRunner agentRunner, string rootDomain, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(rootDomain))
            {
                return false;
            }

            var doWeHaveNewRootDomainToAdd = (agentRunner.RootDomain == null || !rootDomain.Equals(agentRunner.RootDomain.Name, StringComparison.OrdinalIgnoreCase));
            if (!doWeHaveNewRootDomainToAdd)
            {
                return false;
            }

            var existRootDomain = await this.rootDomainService.AnyAsync(r => r.Name == rootDomain && r.Target == agentRunner.Target, cancellationToken);

            return !existRootDomain;
        }

        /// <summary>
        /// Add a new root domain in the target
        /// </summary>
        /// <param name="target">The target to add the new root domain</param>
        /// <param name="rootDomain">The new root domain</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The new root domain added</returns>
        private Task<RootDomain> AddTargetNewRootDomainAsync(Target target, string rootDomain, CancellationToken cancellationToken)
        {
            var newRootDomain = new RootDomain
            {
                Name = rootDomain,
                Target = target
            };

            return this.rootDomainService.AddAsync(newRootDomain, cancellationToken);
        }
    }
}
