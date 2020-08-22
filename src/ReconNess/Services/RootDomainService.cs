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
        /// <see cref="IRootDomainService.SaveScriptOutputAsync(AgentRun, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveScriptOutputAsync(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.UnitOfWork.BeginTransaction();

                var subdomain = agentRun.Subdomain;
                if (!string.IsNullOrEmpty(scriptOutput.Subdomain) &&
                    (subdomain == null || !scriptOutput.Subdomain.Equals(subdomain.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    subdomain = await this.AddOrUpdateSubdomainAsync(agentRun, scriptOutput, cancellationToken);
                }

                if (subdomain != null)
                {
                    await this.subdomainService.UpdateSubdomainByAgentRunning(subdomain, agentRun, scriptOutput, cancellationToken);
                }

                await this.UnitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                this.UnitOfWork.Rollback();
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
            var subdomains = this.SplitSubdomains(uploadSubdomains);

            foreach (var subdomain in subdomains)
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
        /// <param name="agentRun">The agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns>A Task</returns>
        private async Task<Subdomain> AddOrUpdateSubdomainAsync(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (Uri.CheckHostName(scriptOutput.Subdomain) == UriHostNameType.Unknown)
            {
                return null;
            }

            var subdomain = await this.subdomainService.GetAllQueryableByCriteria(d => d.Name == scriptOutput.Subdomain && d.RootDomain == agentRun.RootDomain)
                                .Include(s => s.Services)
                                .FirstOrDefaultAsync();

            if (subdomain == null)
            {
                subdomain = new Subdomain
                {
                    Name = scriptOutput.Subdomain,
                    RootDomain = agentRun.RootDomain
                };

                subdomain = await this.subdomainService.AddAsync(subdomain);

                if (agentRun.ActivateNotification && agentRun.Agent.NotifyNewFound && agentRun.Agent.AgentNotification != null && !string.IsNullOrEmpty(agentRun.Agent.AgentNotification.SubdomainPayload))
                {
                    var payload = agentRun.Agent.AgentNotification.SubdomainPayload.Replace("{{domain}}", subdomain.Name);
                    await this.notificationService.SendAsync(payload, cancellationToken);
                }
            }

            return subdomain;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadSubdomains"></param>
        /// <returns></returns>
        private List<string> SplitSubdomains(IEnumerable<string> uploadSubdomains)
        {
            var subdomains = new List<string>();
            foreach (var subdomain in uploadSubdomains)
            {
                if (subdomain.Contains(","))
                {
                    subdomains.AddRange(subdomain.Split(","));
                }
                else
                {
                    subdomains.Add(subdomain);
                }
            }

            return subdomains;
        }
    }
}