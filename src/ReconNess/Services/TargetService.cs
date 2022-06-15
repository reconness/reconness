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
    /// This class implement <see cref="ITargetService"/>
    /// </summary>
    public class TargetService : Service<Target>, IService<Target>, ITargetService, ISaveTerminalOutputParseService<Target>
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IRootDomainService rootDomainService;
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ITargetService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="notificationService"><see cref="INotificationService"/></param>
        public TargetService(IUnitOfWork unitOfWork,
            IRootDomainService rootDomainService,
            INotificationService notificationService)
            : base(unitOfWork)
        {
            this.rootDomainService = rootDomainService;
            this.notificationService = notificationService;
        }

        /// <inheritdoc/>
        public async Task<List<Target>> GetTargetsNotTrackingAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken)
        {
            return await this.GetAllQueryableByCriteria(predicate)
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
        }

        /// <inheritdoc/>
        public async Task<Target> GetTargetNotTrackingAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken)
        {
            return await this.GetAllQueryableByCriteria(predicate)
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
        }

        /// <inheritdoc/>
        public async Task<Target> GetTargetAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken)
        {
            return await this.GetAllQueryableByCriteria(predicate)
                        .Include(t => t.RootDomains)
                        .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAgentRanAsync(Target target, string agentName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(target.AgentsRanBefore))
            {
                target.AgentsRanBefore = agentName;
                await this.UpdateAsync(target, cancellationToken);
            }
            else if (!target.AgentsRanBefore.Contains(agentName))
            {
                target.AgentsRanBefore = string.Join(", ", target.AgentsRanBefore, agentName);
                await this.UpdateAsync(target, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async Task SaveTerminalOutputParseAsync(Target target, string agentName, bool activateNotification, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            RootDomain rootDomain = default;
            if (await this.NeedAddNewRootDomain(target, terminalOutputParse.RootDomain, cancellationToken))
            {
                rootDomain = await this.AddTargetNewRootDomainAsync(target, terminalOutputParse.RootDomain, cancellationToken);
                if (activateNotification)
                {
                    await this.notificationService.SendAsync(NotificationType.SUBDOMAIN, new[]
                    {
                        ("{{rootDomain}}", rootDomain.Name)
                    }, cancellationToken);
                }
            }

            if (rootDomain != null)
            {
                await this.rootDomainService.SaveTerminalOutputParseAsync(rootDomain, agentName, activateNotification, terminalOutputParse, cancellationToken);
            }
        }

        public async Task<Target> ExportTargetAsync(Expression<Func<Target, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(criteria)
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
                            Data = eventTrack.Data,
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
                                    Data = eventTrack.Data,
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
                                            Data = eventTrack.Data,
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

        /// <inheritdoc/>
        public async Task<TargetDashboard> GetDashboardAsync(string targetName, CancellationToken cancellationToken = default)
        {
            var dashboard = new TargetDashboard();

            var services = await this.UnitOfWork.Repository<Service>().GetAllByCriteriaAsync(s => s.Subdomain.RootDomain.Target.Name == targetName, cancellationToken);
            var groupPorts = services.GroupBy(s => s.Port);

            dashboard.SubdomainByPort = groupPorts.Select(p => new SubdomainByPort { Port = p.Key, Count = p.Count() }).OrderByDescending(s => s.Count);

            var directories = await this.UnitOfWork.Repository<Directory>().GetAllQueryableByCriteria(d => d.Subdomain.RootDomain.Target.Name == targetName)
                    .Include(d => d.Subdomain)
                .ToListAsync(cancellationToken);

            var groupDirectories = directories.GroupBy(s => s.Subdomain);

            dashboard.SubdomainByDirectories = groupDirectories.Select(p => new SubdomainByDirectories { Subdomain = p.Key.Name, Count = p.Count() }).OrderByDescending(d => d.Count).Take(5);

            var groupByDayOfWeek = this.UnitOfWork.Repository<EventTrack>().GetAllQueryableByCriteria(l => l.CreatedAt > DateTime.UtcNow.AddDays(-7))
                .GroupBy(l => l.CreatedAt.DayOfWeek);

            dashboard.Interactions = groupByDayOfWeek.Select(d => new DashboardEventTrackInteraction { Day = d.Key, Count = d.Count() });

            dashboard.EventTracks = this.UnitOfWork.Repository<EventTrack>().GetAllQueryableByCriteria(l => l.Target.Name == targetName)
                .Select(l => new DashboardEventTrack { Date = l.CreatedAt, Data = l.Data, CreatedBy = l.Username }).Take(10);

            return dashboard;
        }

        /// <summary>
        /// If we need to add a new RootDomain
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="rootDomain">The root domain</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>If we need to add a new RootDomain</returns>
        private async ValueTask<bool> NeedAddNewRootDomain(Target target, string rootDomain, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(rootDomain))
            {
                return false;
            }

            var existRootDomain = await this.rootDomainService.AnyAsync(r => r.Target == target && r.Name == rootDomain, cancellationToken);
            return !existRootDomain;
        }

        /// <summary>
        /// Add a new root domain in the target
        /// </summary>
        /// <param name="target">The target to add the new root domain</param>
        /// <param name="rootDomain">The new root domain</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The new root domain added</returns>
        private Task<RootDomain> AddTargetNewRootDomainAsync(Target target, string rootDomain, CancellationToken cancellationToken)
        {
            var newRootDomain = new RootDomain
            {
                Name = rootDomain,
                Target = target
            };

            return this.rootDomainService.AddAsync(newRootDomain, cancellationToken);
        }
    }
}
