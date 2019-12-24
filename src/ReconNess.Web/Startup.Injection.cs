using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Data.Npgsql;
using ReconNess.Services;
using ReconNess.Web.Auth;

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
            services.AddScoped<ITargetService, TargetService>();

            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<INotesService, NotesService>();
            services.AddScoped<ISubdomainService, SubdomainService>();
            services.AddScoped<ILabelService, LabelService>();
            services.AddScoped<IServiceService, ServiceService>();

            services.AddScoped<IScriptEngineService, ScriptEngineService>();
            services.AddScoped<IConnectorService, ConnectorService>();
        }
    }
}
