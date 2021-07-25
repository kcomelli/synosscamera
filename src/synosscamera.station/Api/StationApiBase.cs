using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core.Abstractions;
using synosscamera.core.Diagnostics;
using synosscamera.core.Extensions;
using synosscamera.core.Infrastructure.Http;
using synosscamera.station.Abstractions;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using synosscamera.station.Model.ApiInfo;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Api
{
    /// <summary>
    /// Base class for Surveillance station APIs
    /// </summary>
    public abstract class StationApiBase : IStationApi
    {
        private readonly IMemoryCacheWrapper _cache;
        private readonly SurveillanceStationSettings _settings;
        private readonly SurveillanceStationClient _client;
        private readonly ILogger _logger;
        private Dictionary<string, ApiDetails> _apiList;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        /// <param name="memoryCache"></param>
        /// <param name="loggerFactory"></param>
        public StationApiBase(SurveillanceStationClient client, IOptions<SurveillanceStationSettings> settings, IMemoryCacheWrapper memoryCache, ILoggerFactory loggerFactory)
        {
            client.CheckArgumentNull(nameof(client));
            memoryCache.CheckArgumentNull(nameof(memoryCache));
            loggerFactory.CheckArgumentNull(nameof(loggerFactory));

            _client = client;
            _cache = memoryCache;
            _settings = settings?.Value ?? new SurveillanceStationSettings();
            _logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }
        /// <summary>
        /// Gets the surveillance station web api client
        /// </summary>
        protected SurveillanceStationClient Client => _client;
        /// <summary>
        /// Gets the station settings
        /// </summary>
        protected SurveillanceStationSettings Settings => _settings;
        /// <summary>
        /// Access the cache
        /// </summary>
        protected IMemoryCacheWrapper Cache => _cache;
        /// <summary>
        /// Logger
        /// </summary>
        protected ILogger Logger => _logger;
        /// <summary>
        /// Api name
        /// </summary>
        public abstract string ApiName { get; }
        /// <inheritdoc/>
        public string SynoToken { get => Client.SynoToken; set => Client.SynoToken = value; }

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
            if (_apiList?.ContainsKey(this.ApiName) == true)
                return Task.FromResult(_apiList[this.ApiName].Path);

            return Task.FromResult(string.Empty);
        }
        /// <summary>
        /// Resolve the api path using the api name
        /// </summary>
        /// <returns></returns>
        protected virtual Task<(int minVersion, int maxVersion)> ResolveVersionsForApi(CancellationToken cancellation = default)
        {
            if (_apiList?.ContainsKey(this.ApiName) == true)
                return Task.FromResult((_apiList[this.ApiName].MinVersion, _apiList[this.ApiName].MaxVersion));

            return Task.FromResult((1, 1));
        }
        /// <inheritdoc/>
        void IStationApi.SetApiList(Dictionary<string, ApiDetails> apiList)
        {
            _apiList = apiList;
        }
    }
}
