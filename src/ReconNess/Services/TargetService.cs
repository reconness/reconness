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
    public class TargetService : Service<Target>, IService<Target>, ITargetService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="ITargetService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public TargetService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <inheritdoc/>
        public async Task<List<Target>> GetTargetsNotTrackingAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken)
        {
            return await GetAllQueryableByCriteria(predicate)
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
            return await GetAllQueryableByCriteria(predicate)
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
            return await GetAllQueryableByCriteria(predicate)
                        .Include(t => t.RootDomains)
                        .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task ImportRootDomainAsync(Target target, RootDomain newRootdomain, CancellationToken cancellationToken = default)
        {
            target.RootDomains.Add(newRootdomain);

            await UpdateAsync(target, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAgentRanAsync(Target target, string agentName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(target.AgentsRanBefore))
            {
                target.AgentsRanBefore = agentName;
                await UpdateAsync(target, cancellationToken);
            }
            else if (!target.AgentsRanBefore.Contains(agentName))
            {
                target.AgentsRanBefore = string.Join(", ", target.AgentsRanBefore, agentName);
                await this.UpdateAsync(target, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async Task<TargetDashboard> GetDashboardAsync(Target target, CancellationToken cancellationToken = default)
        {
            // TODO: refactor this method
            var dashboard = new TargetDashboard();

            var groupPorts = await this.UnitOfWork.Repository<Service>().GetAllQueryableByCriteria(s => s.Subdomain.RootDomain.Target == target)
                .GroupBy(s => s.Port)
                .ToListAsync(cancellationToken);

            var groupDirectories = await this.UnitOfWork.Repository<Directory>().GetAllQueryableByCriteria(d => d.Subdomain.RootDomain.Target == target)
                .GroupBy(s => s.Subdomain)
                .OrderBy(s => s.Count())
                .ToListAsync(cancellationToken);

            dashboard.SubdomainByPort = groupPorts.Select(p => new SubdomainByPort { Port = p.Key, Count = p.Count() });
            dashboard.SubdomainByDirectories = groupDirectories.Select(p => new SubdomainByDirectories { Subdomain = p.Key.Name, Count = p.Count() }).Take(5);

            var groupByDayOfWeek = await this.UnitOfWork.Repository<EventTrack>().GetAllQueryableByCriteria(l => l.CreatedAt > DateTime.Now.AddDays(-7))
                .GroupBy(l => l.CreatedAt.DayOfWeek)
                .ToListAsync(cancellationToken);

            dashboard.Interactions = groupByDayOfWeek.Select(d => new DashboardEventTrackInteraction { Day = d.Key, Count = d.Count() });


            dashboard.EventTracks = await this.UnitOfWork.Repository<EventTrack>().GetAllQueryableByCriteria(l => l.Target == target)
                .Select(l => new DashboardEventTrack { Date = l.CreatedAt, Data = l.Data}).Take(10)
                .ToListAsync(cancellationToken);

            throw new NotImplementedException();
        }

    }
}
