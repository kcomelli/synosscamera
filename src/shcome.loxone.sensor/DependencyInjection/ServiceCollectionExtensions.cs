using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shcome.core.Diagnostics;
using shcome.loxone.sensor.AppContext;
using shcome.loxone.sensor.Config;
using shcome.loxone.sensor.Importer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shcome.loxone.sensor.DependencyInjection
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add common sensors
        /// </summary>
        /// <param name="services">Service collection instance</param>
        /// <param name="configuration">Configuration access</param>
        /// <returns>Service collection instance</returns>
        public static IServiceCollection AddCommonLoxoneSensors(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckArgumentNull(nameof(services));

            services.AddOptions();
            services.Configure<SensorOptions>(configuration.GetSection(nameof(SensorOptions)));

            // add db context
            var connectionString = configuration.GetConnectionString("SensorDbConnection");
            var dbPassword = configuration["DbPassword"];
            connectionString = connectionString.Replace("{%password%}", dbPassword);
            services.AddDbContext<SeonsorAppDbContext>(options => options.UseNpgsql().EnableSensitiveDataLogging(true));

            // add importer services
            services.AddScoped<EnergieImporter>();
            services.AddScoped<LightningImporter>();
            services.AddScoped<PresenceDetectionImporter>();
            services.AddScoped<TemperatureImporter>();

            // add background workers for continous data imports
            services.AddHostedService<ImportBackgroundWorker<EnergieImporter>>();
            services.AddHostedService<ImportBackgroundWorker<LightningImporter>>();
            services.AddHostedService<ImportBackgroundWorker<PresenceDetectionImporter>>();
            services.AddHostedService<ImportBackgroundWorker<TemperatureImporter>>();

            return services;
        }
    }
}
