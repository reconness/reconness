using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReconNess.Data.Npgsql;
using System;
using System.Net;
using System.Threading.Tasks;
using VueCliMiddleware;
namespace ReconNess.Web
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            if (!"Development".Equals(Env.EnvironmentName))
            {
                var pgDatabase = Environment.GetEnvironmentVariable("PostgresDb") ??
                                 Environment.GetEnvironmentVariable("PostgresDb", EnvironmentVariableTarget.User);
                var pgUserName = Environment.GetEnvironmentVariable("PostgresUser") ??
                                 Environment.GetEnvironmentVariable("PostgresUser", EnvironmentVariableTarget.User);
                var pgpassword = Environment.GetEnvironmentVariable("PostgresPassword") ??
                                 Environment.GetEnvironmentVariable("PostgresPassword", EnvironmentVariableTarget.User);

                connectionString = connectionString.Replace("{{database}}", pgDatabase)
                                                   .Replace("{{username}}", pgUserName)
                                                   .Replace("{{password}}", pgpassword);
            }

            services.AddDbContext<ReconNessContext>
            (
                options => options
                    .UseNpgsql(connectionString)
                    .LogTo(Console.WriteLine)
            );

            // Auth
            this.ConfigureAuth(services, Env);

            this.AddDependencyInjection(services);

            services.AddAutoMapper(typeof(Startup));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if ("Development".Equals(Env.EnvironmentName))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages(context =>
            {
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;

                if (response.StatusCode == (int)HttpStatusCode.Unauthorized && !request.Path.Value.StartsWith("/api"))
                {
                    response.Redirect("/Auth/login");
                }

                return Task.CompletedTask;
            });

            app.UseEndpoints(endpoints =>
            {
                // Mapping of endpoints goes here:
                endpoints.MapControllers();
                endpoints.MapHub<ReconNessHub>("/AgentRunLogsHub");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if ("Development".Equals(Env.EnvironmentName))
                {
                    spa.UseVueCli(npmScript: "serve", port: 8080);
                }
            });
        }
    }
}
