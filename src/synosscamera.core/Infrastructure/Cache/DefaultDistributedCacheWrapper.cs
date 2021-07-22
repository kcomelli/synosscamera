using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using synosscamera.core.Abstractions;
using synosscamera.core.Configuration;
using synosscamera.core.Diagnostics;
using synosscamera.core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.core.Infrastructure.Cache
{
    /// <summary>
    /// Distributed cache utility wrapper
    /// </summary>
    public class DefaultDistributedCacheWrapper : IDistributedCacheWrapper
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _defaultSerializerSettings;
        private readonly DistributedCacheParallelizeOptions _parallelizeOptions;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="parallelizeOptions"></param>
        /// <param name="distributedCache"></param>
        public DefaultDistributedCacheWrapper(ILoggerFactory loggerFactory, IOptions<DistributedCacheParallelizeOptions> parallelizeOptions, IDistributedCache distributedCache = null)
        {
            loggerFactory.CheckArgumentNull(nameof(loggerFactory));

            _distributedCache = distributedCache;
            _logger = CreateLogger(loggerFactory);

            _defaultSerializerSettings = GetSerializerSettings();

            if (parallelizeOptions?.Value == null)
                _parallelizeOptions = new DistributedCacheParallelizeOptions();
            else
                _parallelizeOptions = parallelizeOptions.Value;
        }

        /// <summary>
        /// Create a logger for this controller
        /// </summary>
        /// <param name="loggerFactory">Injected logger factory</param>
        /// <returns>A new logger instance to use</returns>
        protected virtual ILogger CreateLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Get serializer settings
        /// </summary>
        /// <returns></returns>
        protected virtual JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                MaxDepth = 3
            };
        }

        /// <inheritdoc/>
        public IDistributedCache DistributedCache => _distributedCache;

        /// <summary>
        /// Enable or disable mulithreaded read/write operations depending on the underlying provider support
        /// </summary>
        public bool EnableMultiThreadedOperations { get; set; } = true;
        /// <summary>
        /// Maximal parallel threads for parallel batch operations
        /// </summary>
        public int MaxParallelThreads { get; set; } = 100;
        /// <summary>
        /// Batch size
        /// </summary>
        public int OperationBatchSize { get; set; } = 10;

        /// <summary>
        /// Called if a cache value has been written sucessfully
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        protected virtual Task OnCachWritten(CacheSettings settings, string key, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task SetDistributedCache<T>(string key, T data, CacheSettings settings, JsonSerializerSettings overrideSerializerSettings = null, ILogger logger = null, CancellationToken token = default)
        {
            if (!settings.Enabled)
            {
                (logger ?? _logger).LogDebug("Data caching is disabled via configurion options.");
                return;
            }

            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = settings.GetAbsoluteExpiration(),
                SlidingExpiration = settings.GetSlidingExpiration()
            };

            var serializerSettings = overrideSerializerSettings ?? _defaultSerializerSettings;

            if (_distributedCache != null)
            {
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(data, Formatting.None, serializerSettings), options, token);
                await OnCachWritten(settings, key, token); ;
            }
            else
            {
                (logger ?? _logger).LogWarning("Distributed cache is not avaiable or configured!");
            }
        }

        /// <inheritdoc/>
        public virtual async Task SetDistributedCache(string key, byte[] data, CacheSettings settings, ILogger logger = null, CancellationToken token = default)
        {
            if (!settings.Enabled)
            {
                (logger ?? _logger).LogDebug("Data caching is disabled via configurion options.");
                return;
            }

            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = settings.GetAbsoluteExpiration(),
                SlidingExpiration = settings.GetSlidingExpiration()
            };


            if (_distributedCache != null)
            {
                await _distributedCache.SetAsync(key, data, options, token);
                await OnCachWritten(settings, key, token); ;
            }
            else
            {
                (logger ?? _logger).LogWarning("Distributed cache is not avaiable or configured!");
            }
        }

        /// <summary>
        /// Called if a cache value has been read sucessfully
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="token"></param>
        protected virtual Task OnCachRead<T>(CacheSettings settings, string key, T value, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task<T> LoadFromDistributedCache<T>(string key, CacheSettings settings, JsonSerializerSettings overrideSerializerSettings = null, ILogger logger = null, CancellationToken token = default)
        {
            if (!settings.Enabled)
            {
                (logger ?? _logger).LogDebug("Data caching is disabled via configurion options.");
                return default(T);
            }

            if (_distributedCache != null)
            {
                var data = await _distributedCache.GetStringAsync(key, token);
                if (data.IsPresent())
                {
                    var serializerSettings = overrideSerializerSettings ?? _defaultSerializerSettings;
                    var oldMaxDepth = serializerSettings.MaxDepth;
                    try
                    {
                        serializerSettings.MaxDepth = null; // clear max depth for deserializing
                        var ret = JsonConvert.DeserializeObject<T>(data, serializerSettings);
                        if (!Equals(ret, default(T)))
                            await OnCachRead(settings, key, ret, token); ;

                        return ret;
                    }
                    finally
                    {
                        serializerSettings.MaxDepth = oldMaxDepth;
                    }
                }
            }
            else
            {
                (logger ?? _logger).LogWarning("Distributed cache is not avaiable or configured!");
            }

            return default(T);
        }

        /// <inheritdoc/>
        public virtual async Task<byte[]> LoadFromDistributedCache(string key, CacheSettings settings, ILogger logger = null, CancellationToken token = default)
        {
            if (!settings.Enabled)
            {
                (logger ?? _logger).LogDebug("Data caching is disabled via configurion options.");
                return null;
            }

            if (_distributedCache != null)
            {
                var ret = await _distributedCache.GetAsync(key, token);
                if (!Equals(ret, default(byte[])))
                    await OnCachRead(settings, key, ret, token); ;
                return ret;
            }
            else
            {
                (logger ?? _logger).LogWarning("Distributed cache is not avaiable or configured!");
            }

            return null;
        }

        /// <summary>
        /// Can be overwritten - called if a key has been removed from cache.
        /// Will only be called if a key was explicetly been removed. Expirations will not trigger this method.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        protected virtual Task OnRemoved(CacheSettings settings, string key, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task RemoveFromDistributedCache(string key, CacheSettings settings, ILogger logger = null, CancellationToken token = default)
        {
            if (!settings.Enabled)
            {
                (logger ?? _logger).LogDebug("Data caching is disabled via configurion options.");
                return;
            }

            if (_distributedCache != null)
            {
                await _distributedCache.RemoveAsync(key, token);
                await OnRemoved(settings, key, token); ;
            }
            else
            {
                (logger ?? _logger).LogWarning("Distributed cache is not avaiable or configured!");
            }
        }

        /// <summary>
        /// Called if a cache is about to be cleared
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <param name="token"></param>
        /// <returns>False if clear action should be cancelled</returns>
        protected virtual Task<bool> OnCacheClearing(string keyPrefix, CancellationToken token = default)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Called if a cache has been cleared
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <param name="token"></param>
        protected virtual Task OnCacheCleared(string keyPrefix, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task ClearDistributedCache(string keyPrefix, ILogger logger = null, CancellationToken token = default)
        {
            if (await OnCacheClearing(keyPrefix, token))
            {
                throw new NotSupportedException();
            }
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public virtual async Task<IDictionary<string, TResult>> ExecuteParallelBatch<TResult, TOperation>(CacheSettings settings, IDictionary<string, TOperation> sourceList, Func<IDistributedCacheWrapper, CacheSettings, string, TOperation, ILogger, CancellationToken, Task<TResult>> operation, ILogger logger = null, CancellationToken token = default)
        {
            if (!settings.Enabled)
            {
                (logger ?? _logger).LogDebug("Data caching is disabled via configurion options.");
                return null;
            }

            if (_distributedCache != null)
            {
                var maxThreads = _parallelizeOptions.MaxParallelThreads;
                var batchSize = _parallelizeOptions.OperationBatchSize;

                if (!_parallelizeOptions.EnableMultiThreadedOperations)
                {
                    // sequentially call and await all tasks
                    return await BatchOperation(settings, null, sourceList, operation, logger, token);
                }
                else
                {
                    logger.LogDebug("Starting distributed cache batch operation with '{numberOfOperations}' tasks using {maxParallelThreads} parallel threads max with a batch size of {pollBatchSize}.",
                        sourceList.Count, maxThreads, batchSize);

                    // define max parallel threads using semaphore
                    using (var semaphore = new SemaphoreSlim(maxThreads))
                    {
                        // split the list in chunks of the configured size
                        var batches = sourceList.Split(batchSize);

                        // create tasks for status update polling
                        var awaitableTasks = batches.Select(operationData => BatchOperation(settings, semaphore, operationData, operation, logger, token));

                        // await all tasks to finish
                        var allResults = (await Task.WhenAll(awaitableTasks)).ToList();
                        var ret = allResults.FirstOrDefault();

                        // merge dictionaries
                        if (ret != null && allResults.Count > 1)
                        {
                            for (int i = 1; i < allResults.Count; i++)
                                ret = ret.Union(allResults[i]).ToDictionary(s => s.Key, s => s.Value);
                        }

                        return ret;
                    }
                }
            }
            else
            {
                (logger ?? _logger).LogWarning("Distributed cache is not avaiable or configured!");
            }

            return null;
        }

        private async Task<IDictionary<string, TResult>> BatchOperation<TResult, TOperation>(CacheSettings settings, SemaphoreSlim semaphore, IDictionary<string, TOperation> sourceData, Func<IDistributedCacheWrapper, CacheSettings, string, TOperation, ILogger, CancellationToken, Task<TResult>> operation, ILogger logger, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var ret = new Dictionary<string, TResult>();

            if (semaphore != null)
                await semaphore.WaitAsync(token);

            try
            {
                foreach (var key in sourceData.Keys)
                {
                    var result = await operation(this, settings, key, sourceData[key], logger, token);
                    ret[key] = result;
                }
            }
            finally
            {
                if (semaphore != null)
                    semaphore.Release();
            }

            return ret;
        }
    }
}
