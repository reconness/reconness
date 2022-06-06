using Microsoft.EntityFrameworkCore;
using NLog;
using ReconNess.Core;
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
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="IRootDomainService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public RootDomainService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <inheritdoc/>
        public async Task<RootDomain> GetRootDomainNoTrackingAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await GetAllQueryableByCriteria(criteria)
                    .Select(rootDomain => new RootDomain
                    {
                        Id = rootDomain.Id,
                        Name = rootDomain.Name
                    })
                    .AsNoTracking()
                    .SingleOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<RootDomain> GetRootDomainWithSubdomainsAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await GetAllQueryableByCriteria(criteria)
                        .Include(r => r.Subdomains)
                    .SingleOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<RootDomain> ExportRootDomainNoTrackingAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await GetAllQueryableByCriteria(criteria)
                    .Select(rootDomain => new RootDomain
                    {
                        Name = rootDomain.Name,
                        AgentsRanBefore = rootDomain.AgentsRanBefore,
                        HasBounty = rootDomain.HasBounty,
                        Notes = rootDomain.Notes,
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
                                Notes = subdomain.Notes,                                
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
                    .SingleOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task DeleteSubdomainsAsync(RootDomain rootDomain, CancellationToken cancellationToken)
        {
            rootDomain.Subdomains.Clear();

            await UpdateAsync(rootDomain, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UploadSubdomainsAsync(RootDomain rootDomain, IEnumerable<string> uploadSubdomains, CancellationToken cancellationToken = default)
        {
            if (rootDomain.Subdomains == null)
            {
                rootDomain.Subdomains = new List<Subdomain>();
            }

            var currentSubdomains = rootDomain.Subdomains.Select(s => s.Name);
            var subdomains = SplitSubdomains(uploadSubdomains);
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

            await UpdateAsync(rootDomain, cancellationToken);
        }

        /// <inheritdoc/>
        public ICollection<RootDomain> GetRootDomains(ICollection<RootDomain> myRootDomains, List<string> newRootDomains, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            myRootDomains = GetIntersectionRootDomainsName(myRootDomains, newRootDomains, cancellationToken);
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

        /// <inheritdoc/>
        public async Task UpdateAgentRanAsync(RootDomain rootDomain, string agentName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(rootDomain.AgentsRanBefore))
            {
                rootDomain.AgentsRanBefore = agentName;
                await UpdateAsync(rootDomain, cancellationToken);
            }
            else if (!rootDomain.AgentsRanBefore.Contains(agentName))
            {
                rootDomain.AgentsRanBefore = string.Join(", ", rootDomain.AgentsRanBefore, agentName);
                await UpdateAsync(rootDomain, cancellationToken);
            }
        }

        /// <summary>
        /// Obtain the names of the rootdomains that interset the old and the new rootdomains
        /// </summary>
        /// <param name="myRootDomains">The list of my RootDomains</param>
        /// <param name="newRootDomains">The list of string RootDomains</param>
        /// <returns>The names of the categorias that interset the old and the new RootDomains</returns>
        private static ICollection<RootDomain> GetIntersectionRootDomainsName(ICollection<RootDomain> myRootDomains, List<string> newRootDomains, CancellationToken cancellationToken)
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
        private static List<string> SplitSubdomains(IEnumerable<string> uploadSubdomains)
        {
            var subdomains = new List<string>();
            foreach (var subdomain in uploadSubdomains)
            {
                if (subdomain.Contains(','))
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