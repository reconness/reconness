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
/// This class implement <see cref="IAgentRepository"/>
/// </summary>
internal class AgentRepository : Repository<Agent>, IAgentRepository, IRepository<Agent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentRepository" /> class
    /// </summary>
    /// <param name="context">The implementation of Database Context <see cref="IDbContext" /></param>
    public AgentRepository(IDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<List<Agent>> GetAgentsAsync(CancellationToken cancellationToken = default)
    {
        var result = GetAllQueryable()
                    .Select(agent => new Agent
                    {
                        Id = agent.Id,
                        Name = agent.Name,
                        LastRun = agent.LastRun,
                        Command = agent.Command,
                        AgentType = agent.AgentType,
                        CreatedBy = agent.CreatedBy,
                        PrimaryColor = agent.PrimaryColor,
                        SecondaryColor = agent.SecondaryColor,
                        Repository = agent.Repository,
                        AgentTrigger = agent.AgentTrigger,
                        Script = agent.Script,
                        Target = agent.Target,
                        Image = agent.Image,
                        ConfigurationFileName = agent.ConfigurationFileName,
                        Categories = agent.Categories.Select(category => new Category
                        {
                            Name = category.Name
                        })
                        .ToList()
                    })
                    .AsNoTracking();

        return await result
                .OrderBy(a => a.Categories.Single().Name)
                .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Agent?> GetAgentAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(criteria)
                    .Select(agent => new Agent
                    {
                        Id = agent.Id,
                        Name = agent.Name,
                        Repository = agent.Repository,
                        Script = agent.Script,
                        Command = agent.Command,
                        AgentType = agent.AgentType,
                        AgentTrigger = agent.AgentTrigger,
                        ConfigurationFileName = agent.ConfigurationFileName,
                        Categories = agent.Categories.Select(category => new Category
                        {
                            Name = category.Name
                        })
                        .ToList()
                    })
                    .AsNoTracking()
                    .SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Agent?> GetAgentWithCategoriesTriggerAndEventsAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(criteria)
                    .Include(a => a.Categories)
                    .Include(a => a.AgentTrigger)
                    .Include(a => a.EventTracks)
                .SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Agent?> GetAgentToRunAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(criteria)
                    .Include(a => a.Categories)
                    .Include(a => a.AgentTrigger)
                .SingleOrDefaultAsync(cancellationToken);
}
