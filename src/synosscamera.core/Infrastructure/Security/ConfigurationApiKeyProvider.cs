using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core.Abstractions;
using synosscamera.core.Configuration;
using synosscamera.core.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.core.Infrastructure.Security
{
    /// <summary>
    /// Configuration based api key provider
    /// </summary>
    public class ConfigurationApiKeyProvider : IApiKeyProvider
    {
        private readonly SynossSettings _settings;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public ConfigurationApiKeyProvider(IOptions<SynossSettings> settings, ILogger<ConfigurationApiKeyProvider> logger)
        {
            logger.CheckArgumentNull(nameof(logger));

            _settings = settings?.Value ?? new SynossSettings();
        }
        /// <inheritdoc/>>
        public Task<(bool valid, ApiKeyData keyData)> VerifyApiKey(string apiKey, CancellationToken cancellationToken = default)
        {
            apiKey.CheckArgumentNull(nameof(apiKey));

            var keyData = _settings.ApiKeys?.FirstOrDefault(k => k.ApiKey == apiKey);

            return Task.FromResult((keyData?.Enabled == true, keyData));
        }
    }
}
