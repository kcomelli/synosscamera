using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Abstractions
{
    /// <summary>
    /// Memory cache wrapper with additional methods and cache clear support
    /// </summary>
    public interface IMemoryCacheWrapper : IDisposable
    {
        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="cacheGroupId">An identifier which cache group should be used. Grouped entries can be deleted with one call because they share a change token cancellation source.</param>
        /// <param name="key">An object identifying the entry.</param>
        /// <returns>The newly created Microsoft.Extensions.Caching.Memory.ICacheEntry instance.</returns>
        ICacheEntry CreateEntry(string cacheGroupId, object key);
        /// <summary>
        /// Get the default options for a defined cache
        /// </summary>
        /// <param name="cacheGroupId">An identifier which cache group should be used. Grouped entries can be deleted with one call because they share a change token cancellation source.</param>
        /// <returns>The cache entry options for this cache</returns>
        MemoryCacheEntryOptions DefaultOptionsOfCache(string cacheGroupId);
        /// <summary>
        ///  Removes the object associated with the given key.
        /// </summary>
        /// <param name="cacheGroupId">An identifier which cache group should be used. Grouped entries can be deleted with one call because they share a change token cancellation source.</param>
        /// <param name="key">An object identifying the entry.</param>
        void Remove(string cacheGroupId, object key);
        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="cacheGroupId">An identifier which cache group should be used. Grouped entries can be deleted with one call because they share a change token cancellation source.<br/>The cachegroup will be used to check if the group cache is enabled or not.</param>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The located value or null.</param>
        /// <returns>True if the key was found.</returns>
        bool TryGetValue(string cacheGroupId, object key, out object value);
        /// <summary>
        /// Clears the cache identified by cache id.
        /// </summary>
        /// <param name="cacheGroupId">Identifier of the cache or NULL if all caches should be cleared</param>
        /// <returns>True if the cache was cleared</returns>
        bool Clear(string cacheGroupId = null);
    }
}
