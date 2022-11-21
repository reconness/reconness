using Microsoft.EntityFrameworkCore;
using NLog;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Data.Npgsql.Helpers;
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
    /// This class implement <see cref="ISubdomainService"/> 
    /// </summary>
    public class SubdomainService : Service<Subdomain>, IService<Subdomain>, ISubdomainService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ILabelService labelService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ISubdomainService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="labelService"><see cref="ILabelService"/></param>
        public SubdomainService(
            IUnitOfWork unitOfWork,
            ILabelService labelService)
            : base(unitOfWork)
        {
            this.labelService = labelService;
        }

        /// <inheritdoc/>
        public async Task<List<Subdomain>> GetSubdomainsNoTrackingAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(predicate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Subdomain> GetSubdomainAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(predicate)
                    .Include(t => t.RootDomain)
                        .ThenInclude(r => r.Target)
                .SingleAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Subdomain> GetSubdomainNoTrackingAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await GetAllQueryableByCriteria(predicate)
                    .Include(t => t.Services)
                    .Include(t => t.Notes)
                    .Include(t => t.Directories)
                    .Include(t => t.Labels)
                    .AsNoTracking()
                .SingleAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<PagedResult<Subdomain>> GetPaginateAsync(RootDomain rootDomain, string query, int page, int limit, CancellationToken cancellationToken = default)
        {
            IQueryable<Subdomain> queryable = default;
            if (string.IsNullOrEmpty(query))
            {
                queryable = GetAllQueryableByCriteria(s => s.RootDomain == rootDomain)
                        .Select(subdomain => new Subdomain
                        {
                            Id = subdomain.Id,
                            Name = subdomain.Name,
                            CreatedAt = subdomain.CreatedAt,
                            IpAddress = subdomain.IpAddress,
                            AgentsRanBefore = subdomain.AgentsRanBefore,
                            HasHttpOpen = subdomain.HasHttpOpen,
                            IsAlive = subdomain.IsAlive,
                            IsMainPortal = subdomain.IsMainPortal,
                            Takeover = subdomain.Takeover,
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
                                        Name = service.Name
                                    }).ToList()
                        })
                    .OrderByDescending(s => s.CreatedAt)
                    .AsNoTracking();
            }
            else
            {
                queryable = GetAllQueryableByCriteria(s => s.RootDomain == rootDomain && s.Name.Contains(query))
                    .Select(subdomain => new Subdomain
                    {
                        Name = subdomain.Name,
                        CreatedAt = subdomain.CreatedAt,
                        IpAddress = subdomain.IpAddress,
                        AgentsRanBefore = subdomain.AgentsRanBefore,
                        HasHttpOpen = subdomain.HasHttpOpen,
                        IsAlive = subdomain.IsAlive,
                        IsMainPortal = subdomain.IsMainPortal,
                        Takeover = subdomain.Takeover,
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
                                        Name = service.Name
                                    }).ToList()
                    })
                    .OrderByDescending(s => s.CreatedAt)
                    .AsNoTracking();
            }

            return await queryable.GetPageAsync<Subdomain>(page, limit, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Subdomain> GetWithLabelsAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await GetAllQueryableByCriteria(predicate)
                    .Include(t => t.Labels)
                    .Include(t => t.RootDomain)
                        .ThenInclude(r => r.Target)
                .SingleAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task AddLabelAsync(Subdomain subdomain, string newLabel, CancellationToken cancellationToken = default)
        {
            var myLabels = subdomain.Labels.Select(l => l.Name).ToList();
            myLabels.Add(newLabel);

            subdomain.Labels = await labelService.GetLabelsAsync(subdomain.Labels, myLabels, cancellationToken);

            await UpdateAsync(subdomain, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAgentRanAsync(Subdomain subdomain, string agentName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(subdomain.AgentsRanBefore))
            {
                subdomain.AgentsRanBefore = agentName;
                await UpdateAsync(subdomain, cancellationToken);
            }
            else if (!subdomain.AgentsRanBefore.Contains(agentName))
            {
                subdomain.AgentsRanBefore = string.Join(", ", subdomain.AgentsRanBefore, agentName);
                await UpdateAsync(subdomain, cancellationToken);
            }
        }
    }
}
