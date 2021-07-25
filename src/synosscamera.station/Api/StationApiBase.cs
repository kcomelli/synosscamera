using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core.Abstractions;
using synosscamera.core.Diagnostics;
using synosscamera.core.Extensions;
using synosscamera.core.Infrastructure.Http;
using synosscamera.core.Model.Dto;
using synosscamera.station.Abstractions;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using synosscamera.station.Model;
using synosscamera.station.Model.ApiInfo;
using System.Collections.Generic;
using System.Net;
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
        private readonly Dictionary<int, string> _apiErrorResolval = new Dictionary<int, string>();

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

            BuildCommonApiErrorCodeMappings();
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
        /// Error code to message mappings
        /// </summary>
        protected Dictionary<int, string> ErrorCodeMappings => _apiErrorResolval;

        private void BuildCommonApiErrorCodeMappings()
        {
            _apiErrorResolval[StationConstants.ErrorCodes.Common.UnknownError] = "Unknown error occured.";
            _apiErrorResolval[StationConstants.ErrorCodes.Common.InvalidParameters] = "Invalid parameter and/or value supplied.";
            _apiErrorResolval[StationConstants.ErrorCodes.Common.ApiDoesNotExist] = "The requested api does not exist.";
            _apiErrorResolval[StationConstants.ErrorCodes.Common.MethodDoesNotExist] = "The called method does not exist.";
            _apiErrorResolval[StationConstants.ErrorCodes.Common.ApiVersionNotSupported] = "This version of the api is not supported.";
            _apiErrorResolval[StationConstants.ErrorCodes.Common.InsufficientUserPriviliges] = "User has insufficient priviliges.";
            _apiErrorResolval[StationConstants.ErrorCodes.Common.ConnectionTimeout] = "Connection timed out.";
            _apiErrorResolval[StationConstants.ErrorCodes.Common.MultipleLoginDetected] = "Multiple logins of this user detected.";

            BuildApiErrorCodeMappings();
        }

        /// <summary>
        /// Build error code to text mappings
        /// </summary>
        protected abstract void BuildApiErrorCodeMappings();

        /// <summary>
        /// Get the api call query string
        /// </summary>
        /// <param name="method">Method to call</param>
        /// <param name="action">Action to call if any</param>
        /// <param name="version">Version to use</param>
        /// <param name="parameter">Additional parameters for querystring</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>A relative url including the api path</returns>
        protected async Task<(string action, string query)> GetUrl(string method, string action = null, int version = 1, Dictionary<string, object> parameter = null, CancellationToken cancellation = default)
        {
            var path = await ResolvePathForApi(cancellation);
            var versions = await ResolveVersionsForApi(cancellation);

            if (parameter == null)
                parameter = new Dictionary<string, object>();

            var uriAction = string.Empty.AppendEndpoint(path);

            parameter["api"] = ApiName;
            parameter["method"] = method;
            if (action.IsPresent())
                parameter["action"] = action;

            parameter["version"] = version;

            if (_settings.SessionNameForStation.IsPresent())
                parameter["session"] = _settings.SessionNameForStation;

            return (uriAction, RestApiClientBase.CreateQueryString(parameter));
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
        /// <summary>
        /// Form ApiErrorResponse from station error
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        protected virtual (ApiErrorResponse error, HttpStatusCode statusCode) ErrorResponseFromStationError(StationError error)
        {
            var ret = new ApiErrorResponse();
            var apiError = new ApiError();
            var statusCode = HttpStatusCode.FailedDependency;

            ret.IsApiError = false;
            ret.Errors = new ApiError[] { apiError };

            apiError.ErrorCode = "undefined";
            apiError.ErrorMessage = "Undefined";

            if (error != null)
            {
                var message = "";
                ErrorCodeMappings.TryGetValue(error.Code, out message);

                apiError.ExternalErrorMessage = message;
                apiError.ExternalErrorCode = error.Code.ToString();

                // TODO: mao code to internal errors and adjust HttpStatus
                apiError.ErrorMessage = $"SurveillanceStation API error: {message}";
                apiError.ErrorCode = error.Code.ToString();
                apiError.ErrorSource = "SurveillanceStation";
                apiError.ErrorType = ApiErrorTypes.ExternalRestApi;

                switch (error.Code)
                {
                    case StationConstants.ErrorCodes.Common.ConnectionTimeout:
                        statusCode = HttpStatusCode.RequestTimeout;
                        break;
                    case StationConstants.ErrorCodes.Common.ApiDoesNotExist:
                    case StationConstants.ErrorCodes.Common.MethodDoesNotExist:
                    case StationConstants.ErrorCodes.Common.ApiVersionNotSupported:
                        statusCode = HttpStatusCode.NotImplemented;
                        break;
                }
            }

            return (ret, statusCode);
        }
    }
}
