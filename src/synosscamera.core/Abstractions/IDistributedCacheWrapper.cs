using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using synosscamera.core.Infrastructure.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.core.Abstractions
{
    /// <summary>
    /// Distributed cache wrapper with some useful methods
    /// </summary>
    public interface IDistributedCacheWrapper
    {
        /// <summary>
        /// Gets the distributed cache instance accosiated with the wrapper
        /// </summary>
        IDistributedCache DistributedCache { get; }

        /// <summary>
        /// Set distributed cache entry using json serialized string
        /// </summary>
        /// <typeparam name="T">Type of object to save</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="data">Data to store</param>
        /// <param name="settings">Cache settings</param>
        /// <param name="overrideSerializerSettings">Override serializer settings. If null, default settings will be used</param>
        /// <param name="logger">Logger. if null default logger will be used</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task SetDistributedCache<T>(string key, T data, CacheSettings settings, JsonSerializerSettings overrideSerializerSettings = null, ILogger logger = null, CancellationToken token = default);
        /// <summary>
        /// Set distributed cache entry using binary data
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="data">Data to store</param>
        /// <param name="settings">Cache settings</param>
        /// <param name="logger">Logger. if null default logger will be used</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task SetDistributedCache(string key, byte[] data, CacheSettings settings, ILogger logger = null, CancellationToken token = default);

        /// <summary>
        /// Load a json data object from distributed cache and deserializes to the specified type
        /// </summary>
        /// <typeparam name="T">Type of saved object</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="settings">Cache settings</param>
        /// <param name="overrideSerializerSettings">Override serializer settings. If null, default settings will be used</param>
        /// <param name="logger">Logger. if null default logger will be used</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>The deserialized object</returns>
        Task<T> LoadFromDistributedCache<T>(string key, CacheSettings settings, JsonSerializerSettings overrideSerializerSettings = null, ILogger logger = null, CancellationToken token = default);
        /// <summary>
        /// Load a  data object from distributed cache as binary data
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="settings">Cache settings</param>
        /// <param name="logger">Logger. if null default logger will be used</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Raw binary cache data</returns>
        Task<byte[]> LoadFromDistributedCache(string key, CacheSettings settings, ILogger logger = null, CancellationToken token = default);

        /// <summary>
        /// Remove a cache key
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="settings">Cache settings</param>
        /// <param name="logger">Logger. if null default logger will be used</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task RemoveFromDistributedCache(string key, CacheSettings settings, ILogger logger = null, CancellationToken token = default);

        /// <summary>
        /// Clears distributed cache entries or the whole cache
        /// </summary>
        /// <param name="keyPrefix">Key prefix to delete all keys starting with it</param>
        /// <param name="logger">Logger. if null default logger will be used</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task ClearDistributedCache(string keyPrefix, ILogger logger = null, CancellationToken token = default);

        /// <summary>
        /// Execute parallel batch of defined operation on source list and returns a dictionary with operation results for every cache key used
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TOperation"></typeparam>
        /// <param name="settings"></param>
        /// <param name="sourceList"></param>
        /// <param name="operation"></param>
        /// <param name="logger"></param>
        /// <param name="token"></param>
        /// <returns>A dictionary containing all cache keys as keys and operation results.</returns>
        Task<IDictionary<string, TResult>> ExecuteParallelBatch<TResult, TOperation>(CacheSettings settings, IDictionary<string, TOperation> sourceList, Func<IDistributedCacheWrapper, CacheSettings, string, TOperation, ILogger, CancellationToken, Task<TResult>> operation, ILogger logger = null, CancellationToken token = default);
    }
}
