using Microsoft.Extensions.DependencyInjection;
using ReconNess.Application;
using ReconNess.Application.Managers;
using ReconNess.Domain.Entities;

namespace ReconNess.Infrastructure.Managers;

public class AgentsSettingServerManager : IAgentsSettingServerManager
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="IAgentsSettingServerManager" /> class
    /// </summary>
    /// <param name="serviceScopeFactory"><see cref="IServiceScopeFactory"/></param>
    public AgentsSettingServerManager(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    /// <inheritdoc/>
    public async Task<AgentsSetting?> GetAgentSettingAsync(CancellationToken cancellationToken = default)
    {
        using var scope = this.serviceScopeFactory.CreateScope();
        var agentsSettingService = scope.ServiceProvider.GetService<IAgentsSettingService>();

        return (await agentsSettingService!.GetAllAsync(cancellationToken)).FirstOrDefault();
    }
}
