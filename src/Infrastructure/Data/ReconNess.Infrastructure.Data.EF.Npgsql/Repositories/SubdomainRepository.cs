using Microsoft.EntityFrameworkCore;
using ReconNess.Application.DataAccess;
using ReconNess.Application.DataAccess.Repositories;
using ReconNess.Application.Models;
using ReconNess.Domain.Entities;
using ReconNess.Infrastructure.Data.EF.Npgsql.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Repositories;

/// <summary>
/// This class implement <see cref="ISubdomainRepository"/>
/// </summary>
internal class SubdomainRepository : Repository<Subdomain>, ISubdomainRepository, IRepository<Subdomain>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubdomainRepository" /> class
    /// </summary>
    /// <param name="context">The implementation of Database Context <see cref="IDbContext" /></param>
    public SubdomainRepository(IDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<List<Subdomain>> GetSubdomainsAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(predicate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Subdomain?> GetSubdomainWithLabelsAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(predicate)
                .Include(t => t.Labels)
                .Include(t => t.RootDomain)
                    .ThenInclude(r => r.Target)
            .SingleAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Subdomain?> GetSubdomainWithRootDomainAndTargetAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(predicate)
                .Include(t => t.RootDomain)
                    .ThenInclude(r => r.Target)
            .SingleAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<Subdomain>> GetSubdomainsWithRootDomainAndTargetAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) =>
        await GetAllQueryableByCriteria(predicate)
                .Include(t => t.RootDomain)
                    .ThenInclude(r => r.Target)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Subdomain?> GetSubdomainWithNotesAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) =>
        await GetAllQueryableByCriteria(predicate)
                .Include(t => t.Notes)
            .SingleAsync(cancellationToken);

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
    public async Task<Subdomain?> GetSubdomainWithServicesNotesDirAndLabelsAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(predicate)
                .Include(t => t.Services)
                .Include(t => t.Notes)
                .Include(t => t.Directories)
                .Include(t => t.Labels)
                .AsNoTracking()
            .SingleAsync(cancellationToken);
}
