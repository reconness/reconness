using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Data.Npgsql;
using ReconNess.Services;
using ReconNess.Web.Auth;
using ReconNess.Worker;

namespace ReconNess.Web
{
    public partial class Startup
    {
        private void AddDependencyInjection(IServiceCollection services)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ReconNessContext>();
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));

            services.AddSingleton<IJwtFactory, JwtFactory>();

            services.AddScoped<IDbContext>(d => new ReconNessContext(optionsBuilder.Options));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDbContext, ReconNessContext>();

            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<IAgentRunnerService, AgentRunnerService>();
            services.AddScoped<IAgentCategoryService, AgentCategoryService>();
            services.AddScoped<IAgentTypeService, AgentTypeService>();
            services.AddScoped<ITargetService, TargetService>();
            services.AddScoped<IRootDomainService, RootDomainService>();
            services.AddScoped<ISubdomainService, SubdomainService>();            
            services.AddScoped<INotesService, NotesService>();            
            services.AddScoped<ILabelService, LabelService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IReferenceService, ReferenceService>();

            services.AddSingleton<IAgentBackgroundService, AgentBackgroundService>();

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IConnectorService, ConnectorService>();

            services.AddSingleton<IScriptEngineService, ScriptEngineService>();
            services.AddSingleton<IAgentRunnerProvider, WorkerAgentRunnerProvider>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}
