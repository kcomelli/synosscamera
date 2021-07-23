using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using synosscamera.core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Extensions
{
    /// <summary>
    /// Memory cache wrapper extensions
    /// </summary>
    public static class IMemoryCacheWrapperWrapperExtensions
    {
        /// <summary>
        /// Get a value from cache
        /// </summary>
        /// <param name="cache">Cache to use</param>
        /// <param name="cacheId">Cache identifier</param>
        /// <param name="key">Key</param>
        /// <returns>The value of the cached item as object</returns>
        public static object Get(this IMemoryCacheWrapper cache, string cacheId, object key)
        {
            cache.TryGetValue(cacheId, key, out object value);
            return value;
        }
        /// <summary>
        /// Get a typed value from the cache
        /// </summary>
        /// <typeparam name="TItem">Item type to retrieve</typeparam>
        /// <param name="cache">Cache to use</param>
        /// <param name="cacheId">Cache identifier</param>
        /// <param name="key">Key</param>
        /// <returns>A typed value of the cache value</returns>
        public static TItem Get<TItem>(this IMemoryCacheWrapper cache, string cacheId, object key)
        {
            return (TItem)(cache.Get(cacheId, key) ?? default(TItem));
        }

        /// <summary>
        /// Try to get a typed value from the cache
        /// </summary>
        /// <typeparam name="TItem">Item type to retrieve</typeparam>
        /// <param name="cache">Cache to use</param>
        /// <param name="cacheId">Cache identifier</param>
        /// <param name="key">Key</param>
        /// <param name="value">Output parameter for the value</param>
        /// <returns>True if the value could be read</returns>
        public static bool TryGetValue<TItem>(this IMemoryCacheWrapper cache, string cacheId, object key, out TItem value)
        {
            if (cache.TryGetValue(cacheId, key, out object result))
            {
                if (result is TItem item)
                {
                    value = item;
                    return true;
                }
            }

            value = default;
            return false;
        }
        /// <summary>
        /// Set a typed value 
        /// </summary>
        /// <typeparam name="TItem">Type if the item to set</typeparam>
        /// <param name="cache">Cache to use</param>
        /// <param name="cacheId">Cache identifier</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value to store</param>
        /// <returns>The value set</returns>
        public static TItem Set<TItem>(this IMemoryCacheWrapper cache, string cacheId, object key, TItem value)
        {
            var entry = cache.CreateEntry(cacheId, key);
            if (entry != null)
            {
                entry.Value = value;
                entry.Dispose();
            }

            return value;
        }
        /// <summary>
        /// Set a typed value setting an absolute expiration date
        /// </summary>
        /// <typeparam name="TItem">Type if the item to set</typeparam>
        /// <param name="cache">Cache to use</param>
        /// <param name="cacheId">Cache identifier</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value to store</param>
        /// <param name="absoluteExpiration">Expiration offset</param>
        /// <returns>The value set</returns>
        public static TItem Set<TItem>(this IMemoryCacheWrapper cache, string cacheId, object key, TItem value, DateTimeOffset absoluteExpiration)
        {
            var entry = cache.CreateEntry(cacheId, key);
            if (entry != null)
            {
                entry.AbsoluteExpiration = absoluteExpiration;
                entry.Value = value;
                entry.Dispose();
            }

            return value;
        }

        /// <summary>
        /// Set a typed value setting an absolute expiration date
        /// </summary>
        /// <typeparam name="TItem">Type if the item to set</typeparam>
        /// <param name="cache">Cache to use</param>
        /// <param name="cacheId">Cache identifier</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value to store</param>
        /// <param name="absoluteExpirationRelativeToNow">Absolute expiration timespan</param>
        /// <returns>The value set</returns>
        public static TItem Set<TItem>(this IMemoryCacheWrapper cache, string cacheId, object key, TItem value, TimeSpan absoluteExpirationRelativeToNow)
        {
            var entry = cache.CreateEntry(cacheId, key);
            if (entry != null)
            {
                entry.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
                entry.Value = value;
                entry.Dispose();
            }

            return value;
        }

        /// <summary>
        /// Set a typed value setting expiration token
        /// </summary>
        /// <typeparam name="TItem">Type if the item to set</typeparam>
        /// <param name="cache">Cache to use</param>
        /// <param name="cacheId">Cache identifier</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value to store</param>
        /// <param name="expirationToken">Token used for expiry</param>
        /// <returns>The value set</returns>
        public static TItem Set<TItem>(this IMemoryCacheWrapper cache, string cacheId, object key, TItem value, IChangeToken expirationToken)
        {
            var entry = cache.CreateEntry(cacheId, key);
            if (entry != null)
            {
                entry.AddExpirationToken(expirationToken);
                entry.Value = value;
                entry.Dispose();
            }

            return value;
        }

        /// <summary>
        /// Set a typed value setting custom cache options
        /// </summary>
        /// <typeparam name="TItem">Type if the item to set</typeparam>
        /// <param name="cache">Cache to use</param>
        /// <param name="cacheId">Cache identifier</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value to store</param>
        /// <param name="options">Options to use</param>
        /// <returns>Ther value set</returns>
        public static TItem Set<TItem>(this IMemoryCacheWrapper cache, string cacheId, object key, TItem value, MemoryCacheEntryOptions options)
        {
            var entry = cache.CreateEntry(cacheId, key);

            if (entry != null)
            {
                if (options != null)
                    entry.SetOptions(options);

                entry.Value = value;
                entry.Dispose();
            }

            return value;
        }
    }
}
