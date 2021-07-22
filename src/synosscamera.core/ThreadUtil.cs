using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using synosscamera.core.Diagnostics;
using synosscamera.core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.core
{
    /// <summary>
    /// Thread utility class
    /// </summary>
    public static class ThreadUtil
    {
        /// <summary>
        /// Configure thread pooling - load settings from config
        /// </summary>
        /// <param name="configuration">Access app configuration</param>
        /// <param name="logger"></param>
        public static void ConfigureThreadPool(IConfiguration configuration, ILogger logger = null)
        {
            int? newMaxThreads = null;
            int? newMaxIOThreads = null;
            int? newMinThreads = null;
            int? newMinIOThreads = null;

            var maxWorkerThreadSetting = configuration["MaxWorkerThreadsCount"];
            var maxIoworkerThreadSetting = configuration["MaxIOWorkerThreadsCount"];
            var minWorkerThreadSetting = configuration["MinWorkerThreadsCount"];
            var minIoworkerThreadSetting = configuration["MinIOWorkerThreadsCount"];

            if (maxWorkerThreadSetting.IsPresent() && int.TryParse(maxWorkerThreadSetting, out int parsedWorkerMax))
                newMaxThreads = parsedWorkerMax;

            if (maxIoworkerThreadSetting.IsPresent() && int.TryParse(maxIoworkerThreadSetting, out int parsedIOMax))
                newMaxIOThreads = parsedIOMax;

            if (minWorkerThreadSetting.IsPresent() && int.TryParse(minWorkerThreadSetting, out int parsedWorkerMin))
                newMinThreads = parsedWorkerMin;

            if (minIoworkerThreadSetting.IsPresent() && int.TryParse(minIoworkerThreadSetting, out int parsedIOMin))
                newMinIOThreads = parsedIOMin;

            ConfigureThreadPool(newMaxThreads, newMaxIOThreads, newMinThreads, newMinIOThreads, logger);
        }

        /// <summary>
        /// Configure thread pooling
        /// </summary>
        /// <param name="newMaxThreads"></param>
        /// <param name="newMaxIOThreads"></param>
        /// <param name="newMinThreads"></param>
        /// <param name="newMinIOThreads"></param>
        /// <param name="logger"></param>
        public static void ConfigureThreadPool(int? newMaxThreads = null, int? newMaxIOThreads = null, int? newMinThreads = null, int? newMinIOThreads = null, ILogger logger = null)
        {

            // Get the current settings.
            int numberOfProcessors = Environment.ProcessorCount;
            ThreadPool.GetMaxThreads(out int oldMaxWorker, out int oldMaxIOWorker);
            ThreadPool.GetMinThreads(out int oldMinWorker, out int oldMinIOWorker);

            int maxWorker, maxIOWorker, minWorker, minIOWorker;
            maxWorker = oldMaxWorker;
            maxIOWorker = oldMaxIOWorker;
            minWorker = oldMinWorker;
            minIOWorker = oldMinIOWorker;

            if (newMinThreads.HasValue)
            {
                minWorker = newMinThreads.Value;
                if (minWorker < numberOfProcessors)
                {
                    minWorker = numberOfProcessors;
                    logger?.LogWarning("Adjusted min worker count to {minWorker} because it cannot be lower than number of processor which is {processorCount}.", minWorker, numberOfProcessors);
                }
            }

            if (newMinIOThreads.HasValue)
            {
                minIOWorker = newMinIOThreads.Value;
                if (minIOWorker < numberOfProcessors)
                {
                    logger?.LogWarning("Adjusted min IO worker count from {minWorker} to {processorCount} because it cannot be lower than number of processors.", minWorker, numberOfProcessors);
                    minIOWorker = numberOfProcessors;
                }
            }


            if (newMaxThreads.HasValue)
            {
                maxWorker = newMaxThreads.Value;
                if (maxWorker < minWorker)
                {
                    logger?.LogWarning("Adjusted max worker count of {maxWorker} to {minWorker} because it must not be lower than min worker count.", maxWorker, minWorker);
                    maxWorker = minWorker;
                }
            }

            if (newMaxIOThreads.HasValue)
            {
                maxIOWorker = newMaxIOThreads.Value;
                if (maxIOWorker < minIOWorker)
                {
                    logger?.LogWarning("Adjusted max IO worker count of {maxWorker} to {minWorker} because it must not be lower than min worker count.", maxIOWorker, minIOWorker);
                    maxIOWorker = minIOWorker;
                }
            }

            // update max worker count
            ThreadPool.SetMinThreads(minWorker, minIOWorker);
            ThreadPool.SetMaxThreads(maxWorker, maxIOWorker);

            if (oldMaxWorker != maxWorker || oldMaxIOWorker != maxIOWorker)
                logger?.LogWarning("Reconfigured MAX threads from {oldMaxThreads} to {maxThreads} and io completion threads from {oldMaxIOThreads} to {maxIOThreads}.", oldMaxWorker, maxWorker, oldMaxIOWorker, maxIOWorker);
            if (oldMinWorker != minWorker || oldMinIOWorker != minIOWorker)
                logger?.LogWarning("Reconfigured MIN threads from {oldMinThreads} to {minThreads} and io completion threads from {oldMinIOThreads} to {minIOThreads}.", oldMinWorker, minWorker, oldMinIOWorker, minIOWorker);
        }
    }
}
