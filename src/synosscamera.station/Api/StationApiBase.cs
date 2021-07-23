using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core.Diagnostics;
using synosscamera.core.Extensions;
using synosscamera.core.Infrastructure.Http;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Api
{
    /// <summary>
    /// Base class for Surveillance station APIs
    /// </summary>
    public abstract class StationApiBase
    {
        private readonly IDistributedCache _cache;
        private readonly SurveillanceStationSettings _settings;
        private readonly SurveillanceStationClient _client;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        /// <param name="distributedCache"></param>
        /// <param name="loggerFactory"></param>
        public StationApiBase(SurveillanceStationClient client, IOptions<SurveillanceStationSettings> settings, IDistributedCache distributedCache, ILoggerFactory loggerFactory)
        {
            client.CheckArgumentNull(nameof(client));
            distributedCache.CheckArgumentNull(nameof(distributedCache));
            loggerFactory.CheckArgumentNull(nameof(loggerFactory));

            _client = client;
            _cache = distributedCache;
            _settings = settings?.Value ?? new SurveillanceStationSettings();
            _logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }
        /// <summary>
        /// Gets the surveillance station web api client
        /// </summary>
        protected SurveillanceStationClient Client => _client;
        /// <summary>
        /// Access the cache
        /// </summary>
        protected IDistributedCache Cache => _cache;
        /// <summary>
        /// Logger
        /// </summary>
        protected ILogger Logger => _logger;
        /// <summary>
        /// Api name
        /// </summary>
        public abstract string ApiName { get; }

        /// <summary>
        /// Get the api call query string
        /// </summary>
        /// <param name="method">Method to call</param>
        /// <param name="action">Action to call if any</param>
        /// <param name="version">Version to use</param>
        /// <param name="parameter">Additional parameters for querystring</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>A relative url including the api path</returns>
        protected async Task<string> GetUrl(string method, string action = null, int version = 1, Dictionary<string, object> parameter = null, CancellationToken cancellation = default)
        {
            var path = await ResolvePathForApi(cancellation);
            var versions = await ResolveVersionsForApi(cancellation);

            if (parameter == null)
                parameter = new Dictionary<string, object>();

            var ret = string.Empty.AppendEndpoint(path);

            parameter["api"] = ApiName;
            parameter["method"] = method;
            if (action.IsPresent())
                parameter["action"] = action;

            parameter["version"] = version;

            if (_settings.SessionNameForStation.IsPresent())
                parameter["session"] = _settings.SessionNameForStation;

            return RestApiClientBase.CreateQueryString(parameter);
        }
        /// <summary>
        /// Resolve the api path using the api name
        /// </summary>
        /// <returns></returns>
        protected virtual Task<string> ResolvePathForApi(CancellationToken cancellation = default)
        {
            return Task.FromResult(string.Empty);
        }
        /// <summary>
        /// Resolve the api path using the api name
        /// </summary>
        /// <returns></returns>
        protected virtual Task<(int minVersion, int maxVersion)> ResolveVersionsForApi(CancellationToken cancellation = default)
        {
            return Task.FromResult((1, 1));
        }
    }
}
