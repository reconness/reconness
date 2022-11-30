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

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Repositories;

/// <summary>
/// This class implement <see cref="ITargetRepository"/>
/// </summary>
internal class TargetRepository : Repository<Target>, ITargetRepository, IRepository<Target>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TargetRepository" /> class
    /// </summary>
    /// <param name="context">The implementation of Database Context <see cref="IDbContext" /></param>
    public TargetRepository(IDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<Target?> GetTargetAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken) => 
        await GetAllQueryableByCriteria(predicate)
                    .Select(target => new Target
                    {
                        Id = target.Id,
                        Name = target.Name,
                        BugBountyProgramUrl = target.BugBountyProgramUrl,
                        HasBounty = target.HasBounty,
                        IsPrivate = target.IsPrivate,
                        InScope = target.InScope,
                        OutOfScope = target.OutOfScope,
                        RootDomains = target.RootDomains.Select(rootDomain => new RootDomain
                        {
                            Id = rootDomain.Id,
                            Name = rootDomain.Name
                        })
                        .ToList()
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<List<Target>> GetTargetsAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken) => 
        await GetAllQueryableByCriteria(predicate)
                   .Select(target => new Target
                   {
                       Id = target.Id,
                       Name = target.Name,
                       PrimaryColor = target.PrimaryColor,
                       SecondaryColor = target.SecondaryColor,
                       BugBountyProgramUrl = target.BugBountyProgramUrl,
                       InScope = target.InScope,
                       OutOfScope = target.OutOfScope,
                       IsPrivate = target.IsPrivate,
                       RootDomains = target.RootDomains.Select(rootDomain => new RootDomain
                       {
                           Id = rootDomain.Id,
                           Name = rootDomain.Name
                       })
                       .ToList()
                   })
                   .AsNoTracking()
                   .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Target?> GetTargetWithRootdomainsAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken) => 
        await GetAllQueryableByCriteria(predicate)
                    .Include(t => t.RootDomains)
                .FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Target?> GetTargetWithNotesAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken) =>
        await GetAllQueryableByCriteria(predicate)
                    .Include(t => t.Notes)
                .FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Target?> ExportTargetAsync(Expression<Func<Target, bool>> criteria, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(criteria)
                .Select(target => new Target
                {
                    Name = target.Name,
                    AgentsRanBefore = target.AgentsRanBefore,
                    HasBounty = target.HasBounty,
                    OutOfScope = target.OutOfScope,
                    BugBountyProgramUrl = target.BugBountyProgramUrl,
                    InScope = target.InScope,
                    IsPrivate = target.IsPrivate,
                    PrimaryColor = target.PrimaryColor,
                    SecondaryColor = target.SecondaryColor,
                    CreatedAt = target.CreatedAt,
                    Notes = target.Notes.Select(note => new Note
                    {
                        Comment = note.Comment,
                        CreatedBy = note.CreatedBy,
                        CreatedAt = note.CreatedAt,
                    }).ToList(),
                    EventTracks = target.EventTracks.Select(eventTrack => new EventTrack
                    {
                        Description = eventTrack.Description,
                        Username = eventTrack.Username,
                        CreatedAt = eventTrack.CreatedAt,
                    }).ToList(),
                    RootDomains = target.RootDomains
                        .Select(rootDomain => new RootDomain
                        {
                            Name = rootDomain.Name,
                            AgentsRanBefore = rootDomain.AgentsRanBefore,
                            HasBounty = rootDomain.HasBounty,
                            CreatedAt = rootDomain.CreatedAt,
                            EventTracks = target.EventTracks.Select(eventTrack => new EventTrack
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
                                    Notes = subdomain.Notes.Select(note => new Note
                                    {
                                        Comment = note.Comment,
                                        CreatedBy = note.CreatedBy,
                                        CreatedAt = note.CreatedAt,
                                    }).ToList(),
                                    EventTracks = target.EventTracks.Select(eventTrack => new EventTrack
                                    {
                                        Description = eventTrack.Description,
                                        Username = eventTrack.Username,
                                        CreatedAt = eventTrack.CreatedAt,
                                    }).ToList(),
                                    Labels = subdomain.Labels.Select(label => new Label
                                    {
                                        Name = label.Name,
                                        Color = label.Color
                                    })
                                    .ToList(),
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
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
}
