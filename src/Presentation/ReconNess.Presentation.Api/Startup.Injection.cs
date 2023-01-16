using Microsoft.Extensions.DependencyInjection;
using ReconNess.Application;
using ReconNess.Application.DataAccess;
using ReconNess.Application.Managers;
using ReconNess.Application.Models;
using ReconNess.Application.Providers;
using ReconNess.Application.Services;
using ReconNess.Infrastructure.DataAccess;
using ReconNess.Infrastructure.Identity.Auth;
using ReconNess.Infrastructure.Managers;
using ReconNess.Infrastructure.Providers;

namespace ReconNess.Presentation.Api;

public partial class Startup
{
    private void AddDependencyInjection(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddSingleton<IJwtFactory, JwtFactory>();

        services.AddScoped<IDbContext, ReconNessContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<IAgentRunnerService, AgentRunnerService>();
        services.AddScoped<IAgentCategoryService, AgentCategoryService>();
        services.AddScoped<IAgentsSettingService, AgentsSettingService>();
        services.AddScoped<ITargetService, TargetService>();
        services.AddScoped<IEventTrackService, EventTrackService>();
        
        services.AddScoped<IRootDomainService, RootDomainService>();
        services.AddScoped<ISubdomainService, SubdomainService>();
        services.AddScoped<INotesService, NotesService>();
        services.AddScoped<ILabelService, LabelService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IReferenceService, ReferenceService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuthProvider, AuthProvider>();

        services.AddSingleton<IAgentServerManager, AgentServerManager>();
        services.AddSingleton<IAgentsSettingServerManager, AgentsSettingServerManager>();

        services.AddSingleton<IVersionProvider, VersionProvider>();
        services.AddSingleton<ILogsProvider, LogsProvider>();
        services.AddSingleton<INotificationProvider, NotificationProvider>();
        services.AddSingleton<IMarketplaceProvider, MarketplaceProvider>();
        
        services.AddSingleton<IScriptEngineProvider, ScriptEngineProvider>();
        services.AddSingleton<IQueueProvider<AgentRunnerQueue>, AgentRunnerQueueProvider>();
    }
}
