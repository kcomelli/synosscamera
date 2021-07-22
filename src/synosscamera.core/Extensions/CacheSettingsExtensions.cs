using synosscamera.core.Infrastructure.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Extensions
{
    /// <summary>
    /// Cache settings extensions
    /// </summary>
    public static class CacheSettingsExtensions
    {
        /// <summary>
        /// Get absolute expiration time span value according to the configured settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static TimeSpan? GetAbsoluteExpiration(this CacheSettings settings)
        {
            if (settings?.AbsoluteExpiration != null)
            {
                switch (settings.TimeoutUnit)
                {
                    case CacheTimeoutUnit.Milliseconds:
                        return TimeSpan.FromMilliseconds(settings.AbsoluteExpiration.Value);
                    case CacheTimeoutUnit.Seconds:
                        return TimeSpan.FromSeconds(settings.AbsoluteExpiration.Value);
                    case CacheTimeoutUnit.Minutes:
                        return TimeSpan.FromMinutes(settings.AbsoluteExpiration.Value);
                    case CacheTimeoutUnit.Hours:
                        return TimeSpan.FromHours(settings.AbsoluteExpiration.Value);
                }
            }

            return null;
        }

        /// <summary>
        /// Get sliding expiration time span value according to the configured settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static TimeSpan? GetSlidingExpiration(this CacheSettings settings)
        {
            if (settings?.SlidingExpiration != null)
            {
                switch (settings.TimeoutUnit)
                {
                    case CacheTimeoutUnit.Milliseconds:
                        return TimeSpan.FromMilliseconds(settings.SlidingExpiration.Value);
                    case CacheTimeoutUnit.Seconds:
                        return TimeSpan.FromSeconds(settings.SlidingExpiration.Value);
                    case CacheTimeoutUnit.Minutes:
                        return TimeSpan.FromMinutes(settings.SlidingExpiration.Value);
                    case CacheTimeoutUnit.Hours:
                        return TimeSpan.FromHours(settings.SlidingExpiration.Value);
                }
            }

            return null;
        }
    }
}
