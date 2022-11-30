using ReconNess.Application.DataAccess;
using ReconNess.Application.DataAccess.Repositories;
using ReconNess.Application.Models;
using ReconNess.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Services;

/// <summary>
/// This class implement <see cref="ITargetService"/>
/// </summary>
public class TargetService : Service<Target>, IService<Target>, ITargetService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ITargetService" /> class
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
    public TargetService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    /// <inheritdoc/>
    public async Task<List<Target>> GetTargetsAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken) => 
        await UnitOfWork.Repository<ITargetRepository, Target>().GetTargetsAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public async Task<Target?> GetTargetAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken) =>
        await UnitOfWork.Repository<ITargetRepository, Target>().GetTargetAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public async Task<Target?> GetTargetWithNotesAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken = default) =>
        await UnitOfWork.Repository<ITargetRepository, Target>().GetTargetWithNotesAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public async Task<Target?> GetTargetWithRootdomainsAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken) =>
        await UnitOfWork.Repository<ITargetRepository, Target>().GetTargetWithRootdomainsAsync(predicate, cancellationToken);

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
            await UpdateAsync(target, cancellationToken);
        }
    }

    public async Task<Target?> ExportTargetAsync(Expression<Func<Target, bool>> criteria, CancellationToken cancellationToken = default) =>
        await UnitOfWork.Repository<ITargetRepository, Target>().ExportTargetAsync(criteria, cancellationToken);

    /// <inheritdoc/>
    public async Task<TargetDashboard> GetDashboardAsync(string targetName, CancellationToken cancellationToken = default)
    {
        var dashboard = new TargetDashboard();

        var services = await UnitOfWork.Repository<Service>().GetAllByCriteriaAsync(s => s.Subdomain.RootDomain.Target.Name == targetName, cancellationToken);
        var groupPorts = services.GroupBy(s => s.Port);

        dashboard.SubdomainByPort = groupPorts.Select(p => new SubdomainByPort { Port = p.Key, Count = p.Count() }).OrderByDescending(s => s.Count);

        
        var directories = await UnitOfWork.Repository<IDirectoryRepository, Directory>().GetDirectoriesWithSubdoaminsAsync(d => d.Subdomain.RootDomain.Target.Name == targetName, cancellationToken);

        var groupDirectories = directories.GroupBy(s => s.Subdomain);

        dashboard.SubdomainByDirectories = groupDirectories.Select(p => new SubdomainByDirectories { Subdomain = p.Key.Name, Count = p.Count() }).OrderByDescending(d => d.Count).Take(5);

        var groupByDayOfWeek = UnitOfWork.Repository<EventTrack>().GetAllQueryableByCriteria(l => l.CreatedAt > DateTime.UtcNow.AddDays(-7))
            .GroupBy(l => l.CreatedAt.DayOfWeek);

        dashboard.Interactions = groupByDayOfWeek.Select(d => new DashboardEventTrackInteraction { Day = d.Key, Count = d.Count() });

        dashboard.EventTracks = UnitOfWork.Repository<EventTrack>().GetAllQueryableByCriteria(l => l.Target.Name == targetName)
            .Select(l => new DashboardEventTrack { Date = l.CreatedAt, Data = l.Description, CreatedBy = l.Username }).Take(10);
        
        return dashboard;
    }
}
