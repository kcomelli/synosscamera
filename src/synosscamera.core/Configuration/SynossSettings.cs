using synosscamera.core.Abstractions;
using synosscamera.core.Extensions;
using synosscamera.core.Infrastructure.Cache;
using System.Collections.Generic;

namespace synosscamera.core.Configuration
{
    /// <summary>
    /// Application settings class
    /// </summary>
    public class SynossSettings : ICacheSettingsProvider
    {
        /// <summary>
        /// Constreuctor of the class
        /// </summary>
        public SynossSettings()
        {
            CacheSettings = new Dictionary<string, CacheSettings>()
            {
                {
                    Constants.Cache.SettingKeys.ApiKeyAuthenticationCacheKey,
                    new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Minutes, AbsoluteExpiration = 6, SlidingExpiration = 2 }
                },
                {
                    Constants.Cache.SettingKeys.StationApiListCache,
                    new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Hours, AbsoluteExpiration = 24, SlidingExpiration = 6 }
                },
                {
                    Constants.Cache.SettingKeys.StationCameraListCache,
                    new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Minutes, AbsoluteExpiration = 5 }
                }
            };
        }
        /// <summary>
        /// List of API keys
        /// </summary>
        public List<ApiKeyData> ApiKeys { get; set; } = new List<ApiKeyData>();

        /// <summary>
        /// Cache configuration settings
        /// </summary>
        public Dictionary<string, CacheSettings> CacheSettings { get; set; }

        CacheSettings ICacheSettingsProvider.GetSettings(string settingsKey)
        {
            return CacheSettings.EnsureSettings(settingsKey);
        }
    }
}
