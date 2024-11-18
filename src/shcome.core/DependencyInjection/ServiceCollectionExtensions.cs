using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using shcome.core.Abstractions;
using shcome.core.Configuration;
using shcome.core.Diagnostics;
using shcome.core.Infrastructure.Cache;
using shcome.core.Infrastructure.Security;

namespace shcome.core.DependencyInjection
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Default core components
        /// </summary>
        /// <param name="services">Service collection instance</param>
        /// <param name="configuration">Configuration access</param>
        /// <returns>Service collection instance</returns>
        public static IServiceCollection AddCoreComponents(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckArgumentNull(nameof(services));

            services.AddOptionsAndSettings(configuration);
            services.AddDefaultDistributedCacheWrapper();
            services.AddDefaultMemoryCacheWrapper();

            services.TryAddSingleton<IApiKeyProvider, ConfigurationApiKeyProvider>();

            return services;
        }

        /// <summary>
        /// Add options and default settings
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">Configuration access</param>
        /// <returns></returns>
        public static IServiceCollection AddOptionsAndSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckArgumentNull(nameof(services));

            services.AddOptions();
            services.Configure<ShcomeSettings>(configuration.GetSection(nameof(ShcomeSettings)));

            services.AddSingleton<ICacheSettingsProvider>((prov) =>
            {
                var ctx = prov.GetService<IOptions<ShcomeSettings>>();
                return ctx?.Value;
            });

            return services;
        }

        /// <summary>
        /// Add default distributed cache wrapper
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultDistributedCacheWrapper(this IServiceCollection services)
        {
            services.CheckArgumentNull(nameof(services));

            services.TryAddSingleton<IDistributedCacheWrapper, DefaultDistributedCacheWrapper>();

            return services;
        }
        /// <summary>
        /// Add default memory cache wrapper
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultMemoryCacheWrapper(this IServiceCollection services)
        {
            services.CheckArgumentNull(nameof(services));

            services.TryAddSingleton<IMemoryCacheWrapper, DefaultMemoryCacheWrapper>();

            return services;
        }
    }
}
