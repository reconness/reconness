using Microsoft.EntityFrameworkCore;
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
    /// This class implement <see cref="IRootDomainService"/>
    /// </summary>
    public class RootDomainService : Service<RootDomain>, IService<RootDomain>, IRootDomainService
    {
        private readonly ISubdomainService subdomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootDomainService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public RootDomainService(IUnitOfWork unitOfWork,
            ISubdomainService subdomainService)
            : base(unitOfWork)
        {
            this.subdomainService = subdomainService;
        }

        /// <summary>
        /// <see cref="IRootDomainService.GetDomainWithSubdomainsAsync(Expression{Func{RootDomain, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<RootDomain> GetDomainWithSubdomainsAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default)
        {
            var rootDomain = await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                .Include(t => t.Notes)
                .FirstOrDefaultAsync();

            if (rootDomain != null)
            {
                rootDomain.Subdomains = await this.subdomainService.GetSubdomainsByTargetAsync(rootDomain, cancellationToken);
            }

            return rootDomain;
        }

        /// <summary>
        /// <see cref="IRootDomainService.SaveScriptOutputAsync(RootDomain, Subdomain, Agent, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveScriptOutputAsync(RootDomain domain, Subdomain subdomain, Agent agent, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (subdomain != null)
            {
                try
                {
                    this.UnitOfWork.BeginTransaction();

                    await this.subdomainService.UpdateSubdomain(subdomain, agent, scriptOutput);

                    await this.UnitOfWork.CommitAsync();
                }
                catch (Exception)
                {
                    this.UnitOfWork.Rollback();
                }
            }

            if (subdomain == null || (subdomain != null && !subdomain.Name.Equals(scriptOutput.Subdomain, StringComparison.OrdinalIgnoreCase)))
            {
                await this.AddOrUpdateSubdomainAsync(domain, agent, scriptOutput);
            }
        }

        /// <summary>
        /// <see cref="IRootDomainService.DeleteDomainAsync(RootDomain, CancellationToken)(Target, CancellationToken)"/>
        /// </summary>
        public async Task DeleteDomainAsync(RootDomain domain, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.UnitOfWork.BeginTransaction(cancellationToken);

                this.subdomainService.DeleteSubdomains(domain.Subdomains, cancellationToken);
                this.UnitOfWork.Repository<RootDomain>().Delete(domain, cancellationToken);

                await this.UnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                this.UnitOfWork.Rollback(cancellationToken);
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="IRootDomainService.DeleteSubdomainsAsync(RootDomain, CancellationToken)"/>
        /// </summary>
        public async Task DeleteSubdomainsAsync(RootDomain target, CancellationToken cancellationToken)
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
        /// <see cref="IRootDomainService.UploadSubdomainsAsync(RootDomain, IEnumerable<UploadSubdomain>, CancellationToken)"/>
        /// </summary>
        public async Task<List<Subdomain>> UploadSubdomainsAsync(RootDomain target, IEnumerable<string> uploadSubdomains, CancellationToken cancellationToken = default)
        {
            if (target.Subdomains == null)
            {
                target.Subdomains = new List<Subdomain>();
            }

            var newSubdomains = new List<Subdomain>();
            foreach (var subdomain in uploadSubdomains)
            {
                if (Uri.CheckHostName(subdomain) != UriHostNameType.Unknown && !target.Subdomains.Any(s => s.Name == subdomain))
                {
                    newSubdomains.Add(new Subdomain
                    {
                        Name = subdomain
                    });
                }
            }
            if (newSubdomains.Count > 0)
            {
                foreach (var subdomain in newSubdomains)
                {
                    target.Subdomains.Add(subdomain);
                }

                await this.UpdateAsync(target);
            }

            return newSubdomains;
        }

        /// <summary>
        /// Add or update the subdomain belong to the target
        /// </summary>
        /// <param name="domain">The domain</param>
        /// <param name="agent">The agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns>A Task</returns>
        private async Task AddOrUpdateSubdomainAsync(RootDomain domain, Agent agent, ScriptOutput scriptOutput)
        {
            if (Uri.CheckHostName(scriptOutput.Subdomain) == UriHostNameType.Unknown)
            {
                return;
            }

            var subdomain = await this.subdomainService.GetAllQueryableByCriteria(d => d.Name == scriptOutput.Subdomain && d.Domain == domain)
                                .Include(s => s.Services)
                                .FirstOrDefaultAsync();

            if (subdomain == null)
            {
                subdomain = new Subdomain
                {
                    Name = scriptOutput.Subdomain,
                    Domain = domain
                };

                subdomain = await this.subdomainService.AddAsync(subdomain);
            }

            await this.SaveScriptOutputAsync(domain, subdomain, agent, scriptOutput);
        }
    }
}