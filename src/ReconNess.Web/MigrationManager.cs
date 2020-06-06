using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReconNess.Data.Npgsql;
using System;

namespace ReconNess.Web
{
    /// <summary>
    /// 
    /// </summary>
    public static class MigrationManager
    {
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
                        //Log errors or do anything you think it's needed
                        throw ex;
                    }
                }
            }

            return webHost;
        }
    }
}
