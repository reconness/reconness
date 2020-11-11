using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ReconNess.Data.Npgsql;
using System;

namespace ReconNess.Web
{
    /// <summary>
    /// 
    /// </summary>
    public static class MigrationManager
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webHost"></param>
        /// <returns></returns>
        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<ReconNessContext>())
                {
                    try
                    {
                        appContext.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        throw;
                    }
                }
            }

            return webHost;
        }
    }
}
