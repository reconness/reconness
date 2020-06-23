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
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootDomainService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        /// <param name="notificationService"><see cref="INotificationService"/></param>
        public RootDomainService(IUnitOfWork unitOfWork,
            ISubdomainService subdomainService,
            INotificationService notificationService)
            : base(unitOfWork)
        {
            this.subdomainService = subdomainService;
            this.notificationService = notificationService;
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
        /// <see cref="IRootDomainService.SaveScriptOutputAsync(RootDomain, Subdomain, Agent, ScriptOutput, bool, CancellationToken)"/>
        /// </summary>
        public async Task SaveScriptOutputAsync(RootDomain rootDomain, Subdomain subdomain, Agent agent, ScriptOutput scriptOutput, bool activateNotification, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (subdomain != null)
            {
                try
                {
                    this.UnitOfWork.BeginTransaction();

                    await this.subdomainService.UpdateSubdomain(subdomain, agent, scriptOutput, activateNotification, cancellationToken);

                    await this.UnitOfWork.CommitAsync();
                }
                catch (Exception)
                {
                    this.UnitOfWork.Rollback();
                }
            }

            if (subdomain == null || (subdomain != null && !subdomain.Name.Equals(scriptOutput.Subdomain, StringComparison.OrdinalIgnoreCase)))
            {
                await this.AddOrUpdateSubdomainAsync(rootDomain, agent, scriptOutput, activateNotification, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IRootDomainService.DeleteRootDomains(ICollection<RootDomain>, CancellationToken)(Target, CancellationToken)"/>
        /// </summary>
        public void DeleteRootDomains(ICollection<RootDomain> rootDomains, CancellationToken cancellationToken = default)
        {
            foreach (var rootDomain in rootDomains)
            {
                cancellationToken.ThrowIfCancellationRequested();

                this.subdomainService.DeleteSubdomains(rootDomain.Subdomains, cancellationToken);
                if (rootDomain.Notes != null)
                {
                    this.UnitOfWork.Repository<Note>().Delete(rootDomain.Notes, cancellationToken);
                }

                this.UnitOfWork.Repository<RootDomain>().Delete(rootDomain, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IRootDomainService.DeleteSubdomainsAsync(RootDomain, CancellationToken)"/>
        /// </summary>
        public async Task DeleteSubdomainsAsync(RootDomain rootDomain, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.UnitOfWork.BeginTransaction(cancellationToken);

                this.subdomainService.DeleteSubdomains(rootDomain.Subdomains, cancellationToken);

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
        public async Task<List<Subdomain>> UploadSubdomainsAsync(RootDomain rootDomain, IEnumerable<string> uploadSubdomains, CancellationToken cancellationToken = default)
        {
            if (rootDomain.Subdomains == null)
            {
                rootDomain.Subdomains = new List<Subdomain>();
            }

            var newSubdomains = new List<Subdomain>();
            foreach (var subdomain in uploadSubdomains)
            {
                if (Uri.CheckHostName(subdomain) != UriHostNameType.Unknown && !rootDomain.Subdomains.Any(s => s.Name == subdomain))
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
                    rootDomain.Subdomains.Add(subdomain);
                }

                await this.UpdateAsync(rootDomain);
            }

            return newSubdomains;
        }

        /// <summary>
        /// <see cref="IRootDomainService.UploadRootDomainAsync(RootDomain, RootDomain, CancellationToken)"/>
        /// </summary>
        public async Task<List<Subdomain>> UploadRootDomainAsync(RootDomain rootDomain, RootDomain uploadRootDomain, CancellationToken cancellationToken = default)
        {
            try
            {
                var subdomainsAddd = new List<Subdomain>();                

                if (uploadRootDomain.Subdomains != null)
                {
                    this.UnitOfWork.BeginTransaction(cancellationToken);
                    foreach (var subdomain in uploadRootDomain.Subdomains)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (!rootDomain.Subdomains.Any(s => s.Name == subdomain.Name))
                        {
                            rootDomain.Subdomains.Add(subdomain);
                            subdomainsAddd.Add(subdomain);
                        }
                    }

                    await this.UnitOfWork.CommitAsync(cancellationToken);
                }
                return subdomainsAddd;
            }
            catch (Exception ex)
            {
                this.UnitOfWork.Rollback(cancellationToken);
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="IRootDomainService.GetRootDomains(ICollection{RootDomain}, List{string}, CancellationToken)"/>
        /// </summary>
        public ICollection<RootDomain> GetRootDomains(ICollection<RootDomain> myRootDomains, List<string> newRootDomains, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            myRootDomains = this.GetIntersectionRootDomainsName(myRootDomains, newRootDomains, cancellationToken);
            foreach (var newRootDomain in newRootDomains)
            {
                if (myRootDomains.Any(r => r.Name == newRootDomain))
                {
                    continue;
                }

                myRootDomains.Add(new RootDomain
                {
                    Name = newRootDomain
                });
            }

            return myRootDomains;
        }       

        /// <summary>
        /// Add or update the subdomain belong to the target
        /// </summary>
        /// <param name="domain">The domain</param>
        /// <param name="agent">The agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns>A Task</returns>
        private async Task AddOrUpdateSubdomainAsync(RootDomain domain, Agent agent, ScriptOutput scriptOutput, bool activateNotification, CancellationToken cancellationToken = default)
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
                if (activateNotification && agent.NotifyNewFound)
                {
                    var payload = agent.NotificationPayload.Replace("{{domain}}", subdomain.Name);
                    await this.notificationService.SendAsync(payload, cancellationToken);
                }
            }

            await this.SaveScriptOutputAsync(domain, subdomain, agent, scriptOutput, activateNotification, cancellationToken);
        }

        /// <summary>
        /// Obtain the names of the rootdomains that interset the old and the new rootdomains
        /// </summary>
        /// <param name="myRootDomains">The list of my RootDomains</param>
        /// <param name="newRootDomains">The list of string RootDomains</param>
        /// <returns>The names of the categorias that interset the old and the new RootDomains</returns>
        private ICollection<RootDomain> GetIntersectionRootDomainsName(ICollection<RootDomain> myRootDomains, List<string> newRootDomains, CancellationToken cancellationToken)
        {
            var myRootDomainsName = myRootDomains.Select(c => c.Name).ToList();
            foreach (var myRootDomainName in myRootDomainsName)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!newRootDomains.Contains(myRootDomainName))
                {
                    myRootDomains.Remove(myRootDomains.First(c => c.Name == myRootDomainName));
                }
            }

            return myRootDomains;
        }        
    }
}