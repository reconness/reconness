using Microsoft.EntityFrameworkCore;
using NLog;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="ITargetService"/>
    /// </summary>
    public class TargetService : Service<Target>, IService<Target>, ITargetService, ISaveTerminalOutputParseService<Target>
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
        /// <see cref="ITargetService.GetTargetsNotTrackingAsync(Expression{Func{Target, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<List<Target>> GetTargetsNotTrackingAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken)
        {
            return this.GetAllQueryableByCriteria(predicate, cancellationToken)
                        .Select(target => new Target
                        {
                            Id = target.Id,
                            Name = target.Name,
                            RootDomains = target.RootDomains.Select(rootDomain => new RootDomain
                            {
                                Name = rootDomain.Name
                            })
                            .ToList()
                        })
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="ITargetService.GetTargetNotTrackingAsync(Expression{Func{Target, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<Target> GetTargetNotTrackingAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken)
        {
            return this.GetAllQueryableByCriteria(predicate, cancellationToken)
                        .Select(target => new Target
                        {
                            Id = target.Id,
                            Name = target.Name,
                            BugBountyProgramUrl = target.BugBountyProgramUrl,
                            HasBounty = target.HasBounty,
                            IsPrivate = target.IsPrivate,
                            InScope = target.InScope,
                            OutOfScope = target.OutOfScope,
                            RootDomains = target.RootDomains.Select(rootDomain => new RootDomain
                            {
                                Name = rootDomain.Name
                            })
                            .ToList()
                        })
                        .AsNoTracking()
                       .SingleAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="ITargetService.GetTargetAsync(Expression{Func{Target, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<Target> GetTargetAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken)
        {
            return this.GetAllQueryableByCriteria(predicate, cancellationToken)
                        .Include(t => t.RootDomains)
                       .SingleAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="IRootDomainService.UploadRootDomainAsync(RootDomain, RootDomain, CancellationToken)"/>
        /// </summary>
        public async Task UploadRootDomainAsync(Target target, RootDomain newRootdomain, CancellationToken cancellationToken = default)
        {            
            target.RootDomains.Add(newRootdomain);

            await this.UpdateAsync(target, cancellationToken);            
        }

        /// <summary>
        /// <see cref="ISaveTerminalOutputParseService.UpdateAgentRanAsync(Target, string, CancellationToken)"/>
        /// </summary>
        public async Task UpdateAgentRanAsync(Target target, string agentName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
        /// <see cref="ISaveTerminalOutputParseService.SaveTerminalOutputParseAsync(Target, bool, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveTerminalOutputParseAsync(Target target, bool activateNotification, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            RootDomain rootDomain = default;
            if (await this.NeedAddNewRootDomain(target, terminalOutputParse.RootDomain, cancellationToken))
            {
                rootDomain = await this.AddTargetNewRootDomainAsync(target, terminalOutputParse.RootDomain, cancellationToken);
                if (activateNotification)
                {
                    await this.notificationService.SendAsync(NotificationType.SUBDOMAIN, new[]
                    {
                        ("{{rootDomain}}", rootDomain.Name)
                    }, cancellationToken);
                }
            }

            if (rootDomain != null)
            {
                await this.rootDomainService.SaveTerminalOutputParseAsync(rootDomain, activateNotification, terminalOutputParse, cancellationToken);
            }
        }

        /// <summary>
        /// If we need to add a new RootDomain
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="rootDomain">The root domain</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>If we need to add a new RootDomain</returns>
        private async Task<bool> NeedAddNewRootDomain(Target target, string rootDomain, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(rootDomain))
            {
                return false;
            }

            var existRootDomain = await this.rootDomainService.AnyAsync(r => r.Target == target && r.Name == rootDomain, cancellationToken);
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
