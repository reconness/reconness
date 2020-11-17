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
    public class RootDomainService : Service<RootDomain>, IService<RootDomain>, IRootDomainService, ISaveTerminalOutputParseService<RootDomain>
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
        /// <see cref="IRootDomainService.GetRootDomainNoTrackingAsync(Expression{Func{RootDomain, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<RootDomain> GetRootDomainNoTrackingAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                    .Select(rootDomain => new RootDomain
                    {
                        Id = rootDomain.Id,
                        Name = rootDomain.Name,
                        Notes = rootDomain.Notes
                    })
                    .AsNoTracking()
                    .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="IRootDomainService.GetRootDomainWithSubdomainsAsync(Expression{Func{RootDomain, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<RootDomain> GetRootDomainWithSubdomainsAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                        .Include(r => r.Subdomains)
                    .SingleOrDefaultAsync();
        }

        /// <summary>
        /// <see cref="IRootDomainService.ExportRootDomainNoTrackingAsync(Expression{Func{RootDomain, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<RootDomain> ExportRootDomainNoTrackingAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                    .Select(rootDomain => new RootDomain
                    {
                        Name = rootDomain.Name,
                        AgentsRanBefore = rootDomain.AgentsRanBefore,
                        HasBounty = rootDomain.HasBounty,
                        Notes = new Note
                        {
                            Notes = rootDomain.Notes.Notes
                        },
                        Subdomains = rootDomain.Subdomains
                            .Select(subdomain => new Subdomain
                            {                                
                                Name = subdomain.Name,
                                AgentsRanBefore = subdomain.AgentsRanBefore,
                                HasBounty = subdomain.HasBounty,
                                HasHttpOpen = subdomain.HasHttpOpen,
                                IpAddress = subdomain.IpAddress,
                                IsAlive = subdomain.IsAlive,
                                IsMainPortal = subdomain.IsMainPortal,
                                Takeover = subdomain.Takeover,
                                Technology = subdomain.Technology,
                                ScreenshotHttpPNGBase64 = subdomain.ScreenshotHttpPNGBase64,
                                ScreenshotHttpsPNGBase64 = subdomain.ScreenshotHttpsPNGBase64,
                                Notes = new Note
                                {
                                    Notes = subdomain.Notes.Notes
                                },
                                Labels = subdomain.Labels
                                    .Select(label => new Label
                                    {
                                        Name = label.Name,
                                        Color = label.Color
                                    })
                                    .ToList(),
                                Services = subdomain.Services
                                    .Select(service => new Service
                                    {
                                        Name = service.Name,
                                        Port = service.Port
                                    }).ToList(),
                                Directories = subdomain.Directories
                                    .Select(directory => new Directory
                                    {
                                        Uri = directory.Uri,
                                        Method = directory.Method,
                                        StatusCode = directory.StatusCode,
                                        Size = directory.Size
                                    }).ToList()
                            })
                            .ToList()
                    })
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
        }

        /// <summary>
        /// <see cref="IRootDomainService.DeleteSubdomainsAsync(RootDomain, CancellationToken)"/>
        /// </summary>
        public async Task DeleteSubdomainsAsync(RootDomain rootDomain, CancellationToken cancellationToken)
        {
            rootDomain.Subdomains.Clear();

            await this.UpdateAsync(rootDomain, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRootDomainService.UploadSubdomainsAsync(RootDomain, IEnumerable<UploadSubdomain>, CancellationToken)"/>
        /// </summary>
        public async Task UploadSubdomainsAsync(RootDomain rootDomain, IEnumerable<string> uploadSubdomains, CancellationToken cancellationToken = default)
        {
            if (rootDomain.Subdomains == null)
            {
                rootDomain.Subdomains = new List<Subdomain>();
            }

            var currentSubdomains = rootDomain.Subdomains.Select(s => s.Name);
            var subdomains = this.SplitSubdomains(uploadSubdomains);
            foreach (var subdomain in subdomains)
            {
                if (Uri.CheckHostName(subdomain) != UriHostNameType.Unknown &&
                    !currentSubdomains.Any(s => s.Equals(subdomain, StringComparison.OrdinalIgnoreCase)))
                {
                    rootDomain.Subdomains.Add(new Subdomain
                    {
                        RootDomain = rootDomain,
                        Name = subdomain
                    });
                }
            }

            await this.UpdateAsync(rootDomain, cancellationToken);
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
        /// <see cref="ISaveTerminalOutputParseService.UpdateAgentRanAsync(RootDomain, string, CancellationToken)"/>
        /// </summary>
        public async Task UpdateAgentRanAsync(RootDomain rootDomain, string agentName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
        /// <see cref="ISaveTerminalOutputParseService.SaveTerminalOutputParseAsync(RootDomain, string, bool, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveTerminalOutputParseAsync(RootDomain rootDomain, string agentName, bool activateNotification, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // if we have a new rootdomain
            if (!string.IsNullOrEmpty(terminalOutputParse.RootDomain) && !rootDomain.Name.Equals(terminalOutputParse.RootDomain))
            {
                rootDomain = new RootDomain
                {
                    Name = terminalOutputParse.RootDomain,
                    Target = rootDomain.Target
                };

                await this.AddAsync(rootDomain, cancellationToken);
            }

            Subdomain subdomain = default;
            if (await this.NeedAddNewSubdomain(rootDomain, terminalOutputParse.Subdomain, cancellationToken))
            {
                subdomain = await this.AddRootDomainNewSubdomainAsync(rootDomain.Target, rootDomain, terminalOutputParse.Subdomain, cancellationToken);
                if (activateNotification)
                {
                    await this.notificationService.SendAsync(NotificationType.SUBDOMAIN, new[]
                    {
                        ("{{domain}}", subdomain.Name)
                    }, cancellationToken);
                }
            }

            if (subdomain != null)
            {
                await this.subdomainService.SaveTerminalOutputParseAsync(subdomain, agentName, activateNotification, terminalOutputParse, cancellationToken);
            }
        }

        /// <summary>
        /// If we need to add a new subdomain
        /// </summary>
        /// <param name="rootDomain">The root domain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>If we need to add a new RootDomain</returns>
        private async Task<bool> NeedAddNewSubdomain(RootDomain rootDomain, string subdomain, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subdomain) || Uri.CheckHostName(subdomain) == UriHostNameType.Unknown)
            {
                return false;
            }

            var existSubdomain = await this.subdomainService.AnyAsync(s => s.RootDomain == rootDomain && s.Name == subdomain, cancellationToken);
            return !existSubdomain;
        }

        /// <summary>
        /// Add a new subdomain in the target
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="rootDomain">The root domain to add the new subdomain</param>
        /// <param name="subdomain">The new root domain</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The new subdomain added</returns>
        private Task<Subdomain> AddRootDomainNewSubdomainAsync(Target target, RootDomain rootDomain, string subdomain, CancellationToken cancellationToken)
        {
            var newSubdomain = new Subdomain
            {
                Name = subdomain,
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