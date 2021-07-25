using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core;
using synosscamera.core.Abstractions;
using synosscamera.core.Extensions;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using synosscamera.station.Model.ApiInfo;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Api
{
    /// <summary>
    /// Surveillance station SYNO.API.Info implementation
    /// </summary>
    public class ApiInfo : StationApiBase
    {
        /// <summary>
        /// ApiList cache key
        /// </summary>
        internal const string ApiListKey = "apilistresponse";

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public ApiInfo(SurveillanceStationClient client, IOptions<SurveillanceStationSettings> settings, IMemoryCacheWrapper memoryCache, ILoggerFactory loggerFactory)
            : base(client, settings, memoryCache, loggerFactory)
        {
        }

        /// <summary>
        /// Api name
        /// </summary>
        public override string ApiName => "SYNO.API.Info";

        /// <summary>
        /// Get API list from station
        /// </summary>
        /// <param name="queryApiFilter">Queryfilter to use: Default: 'SYNO.API.Auth,SYNO.SurveillanceStation.'</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Api response from station</returns>
        public async Task<ApiInfoQueryResponse> GetApisAsync(string queryApiFilter = "SYNO.API.Auth,SYNO.SurveillanceStation.", CancellationToken cancellation = default)
        {
            if (Cache.TryGetValue<ApiInfoQueryResponse>(Constants.Cache.SettingKeys.StationApiListCache, ApiListKey, out ApiInfoQueryResponse resp))
            {
                Logger.LogDebug("Loaded API list from cache.");
                return resp;
            }

            var query = await GetUrl("Query", parameter: new System.Collections.Generic.Dictionary<string, object>()
            {
                { "query", queryApiFilter }
            }, cancellation: cancellation);

            var response = await Client.CallGetApiAsync<ApiInfoQueryResponse>(string.Empty, query, token: cancellation);

            if (response?.Success == true)
            {
                Logger.LogDebug("Retrieved API list form station.");
                Cache.Set<ApiInfoQueryResponse>(Constants.Cache.SettingKeys.StationApiListCache, ApiListKey, response);
                return response;
            }
            else
            {
                Logger.LogDebug("Error loading API list form station with code '{errorCode}'.", response.Error?.Code ?? -1);
                // trhow error
            }

            return response;
        }

        /// <inheritdoc/>
        protected override Task<string> ResolvePathForApi(CancellationToken cancellation = default)
        {
            return Task.FromResult("query.cgi");
        }
        /// <inheritdoc/>
        protected override Task<(int minVersion, int maxVersion)> ResolveVersionsForApi(CancellationToken cancellation = default)
        {
            return Task.FromResult((1, 1));
        }
    }
}
