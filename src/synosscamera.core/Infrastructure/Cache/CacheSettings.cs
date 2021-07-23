using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Infrastructure.Cache
{
    /// <summary>
    /// Caching options timeout unit
    /// </summary>
    public enum CacheTimeoutUnit
    {
        /// <summary>
        /// Milliseconds, should only be used internally
        /// </summary>
        Milliseconds,
        /// <summary>
        /// Seconds
        /// </summary>
        Seconds,
        /// <summary>
        /// Minutes
        /// </summary>
        Minutes,
        /// <summary>
        /// Hours
        /// </summary>
        Hours
    }

    /// <summary>
    /// Cache configuration settings
    /// </summary>
    public class CacheSettings
    {
        /// <summary>
        /// Group identifier or cache group key
        /// </summary>
        [JsonIgnore]
        public string GroupId { get; set; }
        /// <summary>
        /// Cache enabled
        /// </summary>
        public bool Enabled { get; set; } = false;
        /// <summary>
        /// Timeout unit for absolute AND sliding expiration
        /// </summary>
        public CacheTimeoutUnit TimeoutUnit { get; set; } = CacheTimeoutUnit.Minutes;
        /// <summary>
        /// Absolute expiration timeout or null if not expireing
        /// </summary>
        public int? AbsoluteExpiration { get; set; }
        /// <summary>
        /// Sliding expiration timeout or null if not expireing
        /// </summary>
        public int? SlidingExpiration { get; set; }

        /// <summary>
        /// Gets a new instance of an enabled settings object with default values
        /// </summary>
        public static CacheSettings EnabledSettings => new CacheSettings() { Enabled = true };
    }
}
