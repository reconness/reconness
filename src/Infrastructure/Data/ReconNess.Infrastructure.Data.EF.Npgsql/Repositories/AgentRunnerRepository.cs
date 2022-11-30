using Microsoft.EntityFrameworkCore;
using ReconNess.Application.DataAccess;
using ReconNess.Application.DataAccess.Repositories;
using ReconNess.Domain.Entities;
using ReconNess.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Repositories;

/// <summary>
/// This class implement <see cref="IAgentRunnerRepository"/>
/// </summary>
internal class AgentRunnerRepository : Repository<AgentRunner>, IAgentRunnerRepository, IRepository<AgentRunner>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentRunnerRepository" /> class
    /// </summary>
    /// <param name="context">The implementation of Database Context <see cref="IDbContext" /></param>
    public AgentRunnerRepository(IDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<AgentRunner?> GetAgentRunnerAsync(string channel, CancellationToken cancellationToken = default) =>
        await GetAllQueryableByCriteria(a => a.Channel == channel)
                .Include(a => a.Commands)
                    .ThenInclude(c => c.Outputs)
            .FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> RunningAgentsAsync(CancellationToken cancellationToken = default) => 
        await this.GetAllQueryableByCriteria(a => a.Stage == AgentRunnerStage.ENQUEUE || a.Stage == AgentRunnerStage.RUNNING)
                .Select(a => a.Channel)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<int> GetChannelCountAsync(Expression<Func<AgentRunner, bool>> predicate, CancellationToken cancellationToken = default) => 
        await GetAllQueryableByCriteria(predicate).CountAsync(cancellationToken);
}
