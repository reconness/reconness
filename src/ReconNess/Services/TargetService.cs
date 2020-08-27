using Microsoft.EntityFrameworkCore.Internal;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="ITargetService"/>
    /// </summary>
    public class TargetService : Service<Target>, IService<Target>, ITargetService, ISaveTerminalOutputParseService
    {
        private readonly IRootDomainService rootDomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ILabelService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public TargetService(IUnitOfWork unitOfWork, IRootDomainService rootDomainService)
            : base(unitOfWork)
        {
            this.rootDomainService = rootDomainService;
        }

        /// <summary>
        /// <see cref="ITargetService.DeleteTargetAsync(Target, CancellationToken)"/>
        /// </summary>
        public async Task DeleteTargetAsync(Target target, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.UnitOfWork.BeginTransaction(cancellationToken);

                this.rootDomainService.DeleteRootDomains(target.RootDomains, cancellationToken);
                this.UnitOfWork.Repository<Target>().Delete(target, cancellationToken);

                await this.UnitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                this.UnitOfWork.Rollback(cancellationToken);
                throw ex;
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
            }

            await this.rootDomainService.SaveTerminalOutputParseAsync(agentRunner, terminalOutputParse, cancellationToken);
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
            var weHaveRootDomainFromParse = !string.IsNullOrEmpty(rootDomain);
            var weHaveRootDomainToAdd = (agentRunner.RootDomain == null || !rootDomain.Equals(agentRunner.RootDomain.Name, StringComparison.OrdinalIgnoreCase));

            if (weHaveRootDomainFromParse && weHaveRootDomainToAdd)
            {
                return !(await this.rootDomainService.AnyAsync(r => r.Name == rootDomain && r.Target == agentRunner.Target, cancellationToken));
            }

            return false;
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
