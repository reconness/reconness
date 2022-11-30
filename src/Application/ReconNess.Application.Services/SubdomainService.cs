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
/// This class implement <see cref="ISubdomainService"/> 
/// </summary>
public class SubdomainService : Service<Subdomain>, IService<Subdomain>, ISubdomainService
{
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
    public async Task<List<Subdomain>> GetSubdomainsAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) => 
        await UnitOfWork.Repository<ISubdomainRepository, Subdomain>().GetSubdomainsAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public async Task<Subdomain?> GetSubdomainWithRootDomainAndTargetAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) =>
        await UnitOfWork.Repository<ISubdomainRepository, Subdomain>().GetSubdomainWithRootDomainAndTargetAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<Subdomain>> GetSubdomainsWithRootDomainAndTargetAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) =>
        await UnitOfWork.Repository<ISubdomainRepository, Subdomain>().GetSubdomainsWithRootDomainAndTargetAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public async Task<Subdomain?> GetSubdomainWithNotesAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) =>
        await UnitOfWork.Repository<ISubdomainRepository, Subdomain>().GetSubdomainWithNotesAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public async Task<Subdomain?> GetSubdomainWithServicesNotesDirectoriesAndLabelsAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) =>
        await UnitOfWork.Repository<ISubdomainRepository, Subdomain>().GetSubdomainWithServicesNotesDirAndLabelsAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public async Task<PagedResult<Subdomain>> GetPaginateAsync(RootDomain rootDomain, string query, int page, int limit, CancellationToken cancellationToken = default) =>
        await UnitOfWork.Repository<ISubdomainRepository, Subdomain>().GetPaginateAsync(rootDomain, query, page, limit, cancellationToken);

    /// <inheritdoc/>
    public async Task<Subdomain?> GetSubdomainWithLabelsAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default) =>
        await UnitOfWork.Repository<ISubdomainRepository, Subdomain>().GetSubdomainWithLabelsAsync(predicate, cancellationToken);

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
