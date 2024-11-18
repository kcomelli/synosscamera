using shcome.core.Infrastructure.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shcome.core.Abstractions
{
    /// <summary>
    /// Cache settings provider
    /// </summary>
    public interface ICacheSettingsProvider
    {
        /// <summary>
        /// Get cache settings for a cache name or key
        /// </summary>
        /// <param name="settingsKey"></param>
        /// <returns></returns>
        CacheSettings GetSettings(string settingsKey);
    }
}
