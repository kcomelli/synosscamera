using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using synosscamera.core.Abstractions;
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
    /// Default memory cache wrapper
    /// </summary>
    public class DefaultMemoryCacheWrapper : IMemoryCacheWrapper
    {
        private readonly ILogger _logger;
        private bool _disposed = false;
        private readonly IMemoryCache _cache;
        private readonly ICacheSettingsProvider _settingsProvider;

        private const string _cancellationSourceCacheKeyPrefix = "DefaultMemoryCacheWrapper:CTS:";

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="settingsProvider"></param>
        /// <param name="cache"></param>
        public DefaultMemoryCacheWrapper(ILoggerFactory loggerFactory, ICacheSettingsProvider settingsProvider, IMemoryCache cache)
        {
            loggerFactory.CheckArgumentNull(nameof(loggerFactory));
            settingsProvider.CheckArgumentNull(nameof(settingsProvider));
            cache.CheckArgumentNull(nameof(cache));

            _logger = CreateLogger(loggerFactory);
            _cache = cache;
            _settingsProvider = settingsProvider;
        }

        /// <summary>
        /// Indicating if the instance has been disposed
        /// </summary>
        public bool IsDisposed => _disposed;
        /// <inheritdoc/>
        protected IMemoryCache Cache => _cache;

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
        /// Get a cache key prefix for the cancellation token caches
        /// </summary>
        protected virtual string CancellationSourceCacheKeyPrefix => _cancellationSourceCacheKeyPrefix;

        /// <inheritdoc/>
        public bool Clear(string cacheGroupId = null)
        {
            CheckObjectDisposed();
            if (cacheGroupId.IsMissing())
                cacheGroupId = "CompleteCache";

            if (OnCacheClearing(cacheGroupId))
            {
                var cts = (CancellationTokenSource)_cache.Get($"{CancellationSourceCacheKeyPrefix}{cacheGroupId}");
                if (cts != null)
                {
                    // removing this will recreate it if a new set operation is will be done
                    _cache.Remove($"{CancellationSourceCacheKeyPrefix}{cacheGroupId}");
                    cts.Cancel();
                    cts.Dispose();

                    OnCacheCleared(cacheGroupId);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Called if a cache is about to be cleared
        /// </summary>
        /// <param name="cacheGroupId"></param>
        /// <returns>False if clear action should be cancelled</returns>
        protected virtual bool OnCacheClearing(string cacheGroupId)
        {
            return true;
        }

        /// <summary>
        /// Called if a cache has been cleared
        /// </summary>
        /// <param name="cacheGroupId"></param>
        protected virtual void OnCacheCleared(string cacheGroupId)
        {

        }

        /// <inheritdoc/>
        public ICacheEntry CreateEntry(string cacheGroupId, object key)
        {
            CheckObjectDisposed();
            VerifycacheGroupId(cacheGroupId);

            var cacheSettings = SettingsOfCache(cacheGroupId);

            if (!cacheSettings.Enabled)
            {
                _logger.LogDebug("{cacheGroupId} caching is disabled via configurion options.", cacheGroupId);
                return null;
            }

            var cacheCancellations = GetCacheCancellationTokens(cacheGroupId);

            var entry = _cache.CreateEntry(key);

            entry.AbsoluteExpirationRelativeToNow = cacheSettings.GetAbsoluteExpiration();
            entry.SlidingExpiration = cacheSettings.GetAbsoluteExpiration();
            entry.Size = 1;
            cacheCancellations.ForEach(t => entry.AddExpirationToken(new CancellationChangeToken(t)));

            OnCachWritten(cacheGroupId, key);

            return entry;
        }

        /// <summary>
        /// Called if a cache value has been written sucessfully
        /// </summary>
        /// <param name="cacheGroupId"></param>
        /// <param name="key"></param>
        protected virtual void OnCachWritten(string cacheGroupId, object key)
        {

        }

        /// <inheritdoc/>
        public MemoryCacheEntryOptions DefaultOptionsOfCache(string cacheGroupId)
        {
            CheckObjectDisposed();
            VerifycacheGroupId(cacheGroupId);

            var cacheSettings = SettingsOfCache(cacheGroupId);

            return new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = cacheSettings.GetAbsoluteExpiration(),
                SlidingExpiration = cacheSettings.GetAbsoluteExpiration(),
                Size = 1
            };
        }

        /// <inheritdoc/>
        public void Remove(string cacheGroupId, object key)
        {
            CheckObjectDisposed();
            var cacheSettings = SettingsOfCache(cacheGroupId);
            if (!cacheSettings.Enabled)
            {
                _logger.LogDebug("{cacheGroupId} caching is disabled via configurion options.", cacheGroupId);
                return;
            }

            _cache.Remove(key);
            OnRemoved(cacheGroupId, key);
        }

        /// <summary>
        /// Can be overwritten - called if a key has been removed from cache.
        /// Will only be called if a key was explicetly been removed. Expirations will not trigger this method.
        /// </summary>
        /// <param name="cacheGroupId"></param>
        /// <param name="key"></param>
        protected virtual void OnRemoved(string cacheGroupId, object key)
        {

        }

        /// <inheritdoc/>
        public bool TryGetValue(string cacheGroupId, object key, out object value)
        {
            CheckObjectDisposed();
            var cacheSettings = SettingsOfCache(cacheGroupId);
            if (!cacheSettings.Enabled)
            {
                value = null;
                _logger.LogDebug("{cacheGroupId} caching is disabled via configurion options.", cacheGroupId);
                return false;
            }

            var ret = _cache.TryGetValue(key, out value);

            if (ret)
                OnCachRead(cacheGroupId, key, value);

            return ret;
        }

        /// <summary>
        /// Called if a cache value has been read sucessfully
        /// </summary>
        /// <param name="cacheGroupId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected virtual void OnCachRead(string cacheGroupId, object key, object value)
        {

        }

        private static void VerifycacheGroupId(string cacheGroupId)
        {
            if (cacheGroupId.IsMissing() || cacheGroupId == "CompleteCache")
                throw new ArgumentException("Cache id must have a value or its current value is invalid!", nameof(cacheGroupId));
        }

        private CacheSettings SettingsOfCache(string cacheGroupId)
        {
            VerifycacheGroupId(cacheGroupId);

            return _settingsProvider.GetSettings(cacheGroupId);
        }

        private List<CancellationToken> GetCacheCancellationTokens(string cacheGroupId)
        {
            VerifycacheGroupId(cacheGroupId);

            var ctsAll = (CancellationTokenSource)_cache.Get($"{CancellationSourceCacheKeyPrefix}CompleteCache");
            if (ctsAll == null)
            {
                ctsAll = new CancellationTokenSource();
                _cache.Set($"{CancellationSourceCacheKeyPrefix}CompleteCache", ctsAll, new MemoryCacheEntryOptions().SetSize(1)); // never expire
            }

            var ctsCache = (CancellationTokenSource)_cache.Get($"{CancellationSourceCacheKeyPrefix}{cacheGroupId}");
            if (ctsCache == null)
            {
                ctsCache = new CancellationTokenSource();
                // cache cancellation tokens should disappear if all the cache gets cleared
                // all cache entries will listen to both tokens
                _cache.Set($"{CancellationSourceCacheKeyPrefix}{cacheGroupId}", ctsCache, new MemoryCacheEntryOptions().SetSize(1).AddExpirationToken(new CancellationChangeToken(ctsAll.Token))); // never expire
            }

            return new List<CancellationToken>() { ctsAll.Token, ctsCache.Token };
        }

        /// <summary>
        /// Checks if a cache group exists in cache.
        /// </summary>
        /// <remarks>Trying to retreive cancellation tokens for the group. If they exist, the group exists.</remarks>
        /// <param name="cacheGroupId"></param>
        /// <returns></returns>
        protected bool CacheGroupExists(string cacheGroupId)
        {
            CheckObjectDisposed();
            if (cacheGroupId.IsMissing())
                cacheGroupId = "CompleteCache";

            var ctsForGroup = (CancellationTokenSource)_cache.Get($"{CancellationSourceCacheKeyPrefix}{cacheGroupId}");

            return ctsForGroup != null && !ctsForGroup.IsCancellationRequested;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            DisposeInternal();
            _disposed = true;
            // since we use DI, lifetime will be handled by the container - also for injected memory cache
            //_cache.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override this on derived classes to implement additional disposal logic
        /// </summary>
        protected virtual void DisposeInternal()
        {

        }

        /// <summary>
        /// An internal method to check if this instance has been disposed. 
        /// </summary>
        /// <remarks>
        /// Use this method in every method which interacts with the data source.
        /// </remarks>
        protected void CheckObjectDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }
    }
}
