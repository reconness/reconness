using Microsoft.EntityFrameworkCore;
using ReconNess.Application.DataAccess;
using ReconNess.Application.DataAccess.Repositories;
using ReconNess.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Infrastructure.DataAccess.Repositories;

/// <summary>
/// This class implement <see cref="IRootDomainRepository"/>
/// </summary>
internal class RootDomainRepository : Repository<RootDomain>, IRootDomainRepository, IRepository<RootDomain>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RootDomainRepository" /> class
    /// </summary>
    /// <param name="context">The implementation of Database Context <see cref="IDbContext" /></param>
    public RootDomainRepository(IDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<RootDomain?> GetRootDomainAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(criteria)
                .Select(rootDomain => new RootDomain
                {
                    Id = rootDomain.Id,
                    Name = rootDomain.Name
                })
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<RootDomain?> GetRootDomainWithSubdomainsAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(criteria)
                    .Include(r => r.Subdomains)
                .SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<RootDomain>> GetRootDomainsWithTargetsAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default) =>
        await GetAllQueryableByCriteria(criteria)
                    .Include(s => s.Target)
                .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<RootDomain?> GetRootDomainWithNotesAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default) =>
        await GetAllQueryableByCriteria(criteria)
                    .Include(r => r.Notes)
                .SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<RootDomain?> ExportRootDomainAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(criteria)
                .Select(rootDomain => new RootDomain
                {
                    Name = rootDomain.Name,
                    AgentsRanBefore = rootDomain.AgentsRanBefore,
                    HasBounty = rootDomain.HasBounty,
                    CreatedAt = rootDomain.CreatedAt,
                    EventTracks = rootDomain.EventTracks.Select(eventTrack => new EventTrack
                    {
                        Description = eventTrack.Description,
                        Username = eventTrack.Username,
                        CreatedAt = eventTrack.CreatedAt,
                    }).ToList(),
                    Notes = rootDomain.Notes.Select(note => new Note
                    {
                        Comment = note.Comment,
                        CreatedBy = note.CreatedBy,
                        CreatedAt = note.CreatedAt,
                    }).ToList(),
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
                            CreatedAt = subdomain.CreatedAt,
                            EventTracks = subdomain.EventTracks.Select(eventTrack => new EventTrack
                            {
                                Description = eventTrack.Description,
                                Username = eventTrack.Username,
                                CreatedAt = eventTrack.CreatedAt,
                            }).ToList(),
                            Notes = subdomain.Notes.Select(note => new Note
                            {
                                Comment = note.Comment,
                                CreatedBy = note.CreatedBy,
                                CreatedAt = note.CreatedAt,
                            }).ToList(),
                            Labels = subdomain.Labels.Select(label => new Label
                            {
                                Name = label.Name,
                                Color = label.Color
                            }).ToList(),
                            Services = subdomain.Services.Select(service => new Service
                            {
                                Name = service.Name,
                                Port = service.Port
                            }).ToList(),
                            Directories = subdomain.Directories.Select(directory => new Directory
                            {
                                Uri = directory.Uri,
                                Method = directory.Method,
                                StatusCode = directory.StatusCode,
                                Size = directory.Size
                            }).ToList()
                        }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);
}
