using shcome.loxone.sensor.AppContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shcome.loxone.sensor.Migrations
{
    public static class MigrationUtility
    {
        public static void ApplyMigrations(IServiceProvider services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("EanbleMigrations"))
            {
                // aspnet identity + storemind specific tables
                services.ApplyDbMigrations<SeonsorAppDbContext>();
              
            }
        }
        /// <summary>
        /// Apply configuration ef database creation and migrations
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceProvider ApplyDbMigrations<TDbContext>(this IServiceProvider services)
            where TDbContext : DbContext
        {
            var loggerFactory = services.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("data.migrations");

            logger?.LogWarning("Starting database migrations");
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
            {
                logger?.LogWarning("Migrating '{context}' ...", typeof(TDbContext).Name);
                EnsureAndRunMigration<TDbContext>(serviceScope, logger);
            }
            logger?.LogWarning("DONE!");
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="scope"></param>
        /// <param name="logger"></param>
        /// <returns>True if any migrations has been applied</returns>
        private static bool EnsureAndRunMigration<TDbContext>(IServiceScope scope, ILogger logger = null)
           where TDbContext : DbContext
        {
            logger?.LogWarning("Retrieving Db-Context");
            var dbContext = scope.ServiceProvider.GetService<TDbContext>();

            if (dbContext != null)
            {
                // migrations may run a little longer
                dbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(100));

                var migrationsPending = dbContext.Database.GetPendingMigrations();
                if (migrationsPending.Any())
                {
                    logger?.LogWarning("Applying migrations");
                    dbContext.Database.Migrate();

                    dbContext.SaveChanges();

                    var somethingApplied = migrationsPending.Count() != dbContext.Database.GetPendingMigrations().Count();
                    return somethingApplied;
                }
                else
                {
                    logger?.LogWarning("Skip applying migrations - no migrations pending");
                }
            }
            else
            {
                logger?.LogError("Could not retrieve database context");
            }

            return false;
        }
    }
}
