using Microsoft.Extensions.DependencyInjection;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Data.Npgsql;
using ReconNess.Providers;
using ReconNess.Services;
using ReconNess.Web.Auth;

namespace ReconNess.Web
{
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
            services.AddScoped<ITargetService, TargetService>();
            services.AddScoped<IRootDomainService, RootDomainService>();
            services.AddScoped<ISubdomainService, SubdomainService>();
            services.AddScoped<INotesService, NotesService>();
            services.AddScoped<ILabelService, LabelService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IReferenceService, ReferenceService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAuthProvider, AuthProvider>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddSingleton<IVersionProvider, VersionProvider>();
            services.AddSingleton<ILogsProvider, LogsProvider>();

            services.AddSingleton<IScriptEngineService, ScriptEngineService>();
            services.AddSingleton<IQueueProvider<AgentRunnerQueue>, AgentRunnerQueueProvider>();
        }
    }
}
