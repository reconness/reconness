using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="ITargetService"/>
    /// </summary>
    public class TargetService : Service<Target>, IService<Target>, ITargetService
    {
        private readonly ISubdomainService subdomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public TargetService(IUnitOfWork unitOfWork,
            ISubdomainService subdomainService)
            : base(unitOfWork)
        {
            this.subdomainService = subdomainService;
        }

        /// <summary>
        /// <see cref="ITargetService.GetTargetWithSubdomainsAsync(Expression{Func{Target, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<Target> GetTargetWithSubdomainsAsync(Expression<Func<Target, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                .Include(t => t.Notes)
                .Include(t => t.Subdomains)
                    .ThenInclude(t => t.Services)
                .Include(t => t.Subdomains)
                    .ThenInclude(t => t.Notes)
                .Include(t => t.Subdomains)
                    .ThenInclude(a => a.Labels)
                        .ThenInclude(ac => ac.Label)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// <see cref="ITargetService.SaveScriptOutputAsync(Target, Subdomain, Agent, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveScriptOutputAsync(Target target, Subdomain subdomain, Agent agent, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (subdomain != null)
            {
                try
                {
                    this.UnitOfWork.BeginTransaction();

                    await this.subdomainService.UpdateSubdomainAsync(subdomain, agent, scriptOutput, cancellationToken);

                    await this.UnitOfWork.CommitAsync();
                }
                catch (Exception)
                {
                    this.UnitOfWork.Rollback();
                }
            }

            if (subdomain == null || (subdomain != null && !subdomain.Name.Equals(scriptOutput.Subdomain, StringComparison.OrdinalIgnoreCase)))
            {
                await this.AddOrUpdateSubdomainAsync(target, agent, scriptOutput);
            }
        }

        /// <summary>
        /// <see cref="ITargetService.DeleteTargetAsync(Target, CancellationToken)(Target, CancellationToken)"/>
        /// </summary>
        public async Task DeleteTargetAsync(Target target, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.UnitOfWork.BeginTransaction(cancellationToken);

                this.subdomainService.DeleteSubdomains(target.Subdomains, cancellationToken);
                this.UnitOfWork.Repository<Target>().Delete(target, cancellationToken);

                await this.UnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                this.UnitOfWork.Rollback(cancellationToken);
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="ITargetService.DeleteSubdomainsAsync(Target, CancellationToken)"/>
        /// </summary>
        public async Task DeleteSubdomainsAsync(Target target, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.UnitOfWork.BeginTransaction(cancellationToken);

                this.subdomainService.DeleteSubdomains(target.Subdomains, cancellationToken);

                await this.UnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                this.UnitOfWork.Rollback(cancellationToken);
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="ITargetService.UploadSubdomainsAsync(Target, IEnumerable<UploadSubdomain>, CancellationToken)"/>
        /// </summary>
        public async Task UploadSubdomainsAsync(Target target, IEnumerable<string> uploadSubdomains, CancellationToken cancellationToken = default)
        {
            if (target.Subdomains == null)
            {
                target.Subdomains = new List<Subdomain>();
            }

            var hasNewSubdomain = false;
            foreach (var subdomain in uploadSubdomains)
            {
                if (Uri.CheckHostName(subdomain) != UriHostNameType.Unknown && !target.Subdomains.Any(s => s.Name == subdomain))
                {
                    target.Subdomains.Add(new Subdomain
                    {
                        Name = subdomain
                    });

                    hasNewSubdomain = true;
                }
            }
            if (hasNewSubdomain)
            {
                await this.UpdateAsync(target);
            }
        }

        /// <summary>
        /// Add or update the subdomain belong to the target
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="agent">The agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns>A Task</returns>
        private async Task AddOrUpdateSubdomainAsync(Target target, Agent agent, ScriptOutput scriptOutput)
        {
            if (Uri.CheckHostName(scriptOutput.Subdomain) == UriHostNameType.Unknown)
            {
                return;
            }

            var subdomain = await this.subdomainService.GetAllQueryableByCriteria(d => d.Name == scriptOutput.Subdomain && d.Target == target)
                                .Include(s => s.Services)
                                .FirstOrDefaultAsync();

            if (subdomain == null)
            {
                subdomain = new Subdomain
                {
                    Name = scriptOutput.Subdomain,
                    Target = target
                };

                subdomain = await this.subdomainService.AddAsync(subdomain);
            }

            await this.SaveScriptOutputAsync(target, subdomain, agent, scriptOutput);
        }
    }
}