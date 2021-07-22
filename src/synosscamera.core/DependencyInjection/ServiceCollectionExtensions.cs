using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using synosscamera.core.Abstractions;
using synosscamera.core.Diagnostics;
using synosscamera.core.Infrastructure.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.DependencyInjection
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
       
        /// <summary>
        /// Topmind asp.net defaults
        /// </summary>
        /// <param name="services">Service collection instance</param>
        /// <returns>Service collection instance</returns>
        public static IServiceCollection AddsynossCameraDefaults(this IServiceCollection services)
        {
            services.CheckArgumentNull(nameof(services));

            services.AddDefaultDistributedCacheWrapper();
            services.AddDefaultMemoryCacheWrapper();

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
