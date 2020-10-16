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
    /// This class implement <see cref="IRootDomainService"/>
    /// </summary>
    public class RootDomainService : Service<RootDomain>, IService<RootDomain>, IRootDomainService, ISaveTerminalOutputParseService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ISubdomainService subdomainService;
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IRootDomainService" /> class
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
        /// <see cref="IRootDomainService.GetWithSubdomainsAsync(Expression{Func{RootDomain, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<RootDomain> GetWithSubdomainsAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default)
        {
            var rootDomain = await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                    .Include(r => r.Notes)
                    .Include(r => r.Subdomains)
                        .ThenInclude(s => s.Services)
                    .Include(r => r.Subdomains)
                        .ThenInclude(s => s.Labels)
                            .ThenInclude(ac => ac.Label)
                    .FirstOrDefaultAsync();

            return rootDomain;
        }

        /// <summary>
        /// <see cref="IRootDomainService.GetWithIncludeAsync(Expression{Func{RootDomain, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<RootDomain> GetWithIncludeAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default)
        {
            var rootDomain = await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                    .Include(t => t.Target)
                    .Include(t => t.Notes)
                .FirstOrDefaultAsync();

            if (rootDomain != null)
            {
                rootDomain.Subdomains = await this.subdomainService.GetAllWithIncludesAsync(rootDomain.Target, rootDomain, string.Empty, cancellationToken);
            }

            return rootDomain;
        }

        /// <summary>
        /// <see cref="ISaveTerminalOutputParseService.UpdateAgentRanAsync(AgentRunner, CancellationToken)"/>
        /// </summary>
        public async Task UpdateAgentRanAsync(AgentRunner agentRunner, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rootDomain = agentRunner.RootDomain;
            var agentName = agentRunner.Agent.Name;

            if (string.IsNullOrWhiteSpace(rootDomain.AgentsRanBefore))
            {
                rootDomain.AgentsRanBefore = agentName;
                await this.UpdateAsync(rootDomain, cancellationToken);
            }
            else if (!rootDomain.AgentsRanBefore.Contains(agentName))
            {
                rootDomain.AgentsRanBefore = string.Join(", ", rootDomain.AgentsRanBefore, agentName);
                await this.UpdateAsync(rootDomain, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="ISaveTerminalOutputParseService.SaveTerminalOutputParseAsync(AgentRunner, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveTerminalOutputParseAsync(AgentRunner agentRunner, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await this.NeedAddNewSubdomain(agentRunner, terminalOutputParse.Subdomain, cancellationToken))
            {
                agentRunner.Subdomain = await this.AddRootDomainNewSubdomainAsync(agentRunner.Target, agentRunner.RootDomain, terminalOutputParse.Subdomain, cancellationToken);

                if (agentRunner.ActivateNotification)
                {
                    await this.notificationService.SendAsync(NotificationType.SUBDOMAIN, new[]
                    {
                        ("{{domain}}", agentRunner.Subdomain.Name)
                    }, cancellationToken);
                }
            }

            if (agentRunner.Subdomain != null)
            {
                await this.subdomainService.SaveTerminalOutputParseAsync(agentRunner, terminalOutputParse, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IRootDomainService.DeleteSubdomainsAsync(RootDomain, CancellationToken)"/>
        /// </summary>
        public async Task DeleteSubdomainsAsync(RootDomain rootDomain, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            rootDomain.Subdomains = new List<Subdomain>();
            await this.UpdateAsync(rootDomain, cancellationToken);            
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
                        Target = rootDomain.Target,
                        RootDomain = rootDomain,
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

                await this.UpdateAsync(rootDomain, cancellationToken);
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
                _logger.Error(ex, ex.Message);

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
        /// If we need to add a new subdomain
        /// </summary>
        /// <param name="agentRunner">The Agent running</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>If we need to add a new RootDomain</returns>
        private async Task<bool> NeedAddNewSubdomain(AgentRunner agentRunner, string subdomain, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subdomain) || Uri.CheckHostName(subdomain) == UriHostNameType.Unknown)
            {
                return false;
            }

            var weHaveSubdomainToAdd = (agentRunner.Subdomain == null || !subdomain.Equals(agentRunner.Subdomain.Name, StringComparison.OrdinalIgnoreCase));
            if (!weHaveSubdomainToAdd)
            {
                return false;
            }

            var existSubdomain = await this.subdomainService.AnyAsync(r => r.Name == subdomain && r.RootDomain == agentRunner.RootDomain && r.Target == agentRunner.Target, cancellationToken);

            return !existSubdomain;
        }

        /// <summary>
        /// Add a new subdomain in the target
        /// </summary>
        /// <param name="rootDomain">The root domain to add the new subdomain</param>
        /// <param name="subdomain">The new root domain</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The new subdomain added</returns>
        private Task<Subdomain> AddRootDomainNewSubdomainAsync(Target target, RootDomain rootDomain, string subdomain, CancellationToken cancellationToken)
        {
            var newSubdomain = new Subdomain
            {
                Name = subdomain,
                Target = target,
                RootDomain = rootDomain
            };

            return this.subdomainService.AddAsync(newSubdomain, cancellationToken);
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