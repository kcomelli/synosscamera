using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace shcome.loxone.sensor.Extensions
{
    /// <summary>
    /// Configuration extensions
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Get strongly typed configuration data
        /// </summary>
        /// <typeparam name="TSettings">Type of configuration class</typeparam>
        /// <param name="configuration">Configuration root</param>
        /// <returns></returns>
        public static TSettings GetStronglyTypedSettings<TSettings>(this IConfiguration configuration)
            where TSettings : new()
        {
            var settings = new TSettings();
            configuration.GetSection(typeof(TSettings).Name).Bind(settings);
            return settings;
        }
        /// <summary>
        /// Get strongly typed configuration data
        /// </summary>
        /// <typeparam name="TSettings">Type of configuration class</typeparam>
        /// <param name="configuration">Configuration root</param>
        /// <param name="sectionName">Configuration section name</param>
        /// <returns></returns>
        public static TSettings GetStronglyTypedSettings<TSettings>(this IConfiguration configuration, string sectionName)
            where TSettings : new()
        {
            if (string.IsNullOrEmpty(sectionName))
                return configuration.GetStronglyTypedSettings<TSettings>();

            var settings = new TSettings();
            configuration.GetSection(sectionName).Bind(settings);
            return settings;
        }
    }
}
