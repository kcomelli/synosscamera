using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core;
using synosscamera.core.Abstractions;
using synosscamera.core.Extensions;
using synosscamera.station.Abstractions;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using synosscamera.station.Model.ApiInfo;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Api
{
    /// <summary>
    /// Api camera info
    /// </summary>
    public class ApiCamera : AuthenticatedStationApiBase
    {
        /// <summary>
        /// CameraList cache key
        /// </summary>
        internal const string CameraListKey = "cameralistresponse";

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public ApiCamera(SurveillanceStationClient client, IOptions<SurveillanceStationSettings> settings, IMemoryCacheWrapper memoryCache, ILoggerFactory loggerFactory, IStationApiUtil apiUtil)
            : base(client, settings, memoryCache, loggerFactory, apiUtil)
        {
        }

        /// <summary>
        /// Api name
        /// </summary>
        public override string ApiName => StationConstants.Api.ApiCamera.Name;

        /// <inheritdoc/>
        protected override void BuildApiErrorCodeMappings()
        {
        }

        /// <summary>
        /// Get camera list from station
        /// </summary>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Api response from station</returns>
        public async Task<ApiCameraListResponse> GetCameraListAsync(CancellationToken cancellation = default)
        {
            if (Cache.TryGetValue<ApiCameraListResponse>(Constants.Cache.SettingKeys.StationCameraListCache, CameraListKey, out ApiCameraListResponse resp))
            {
                Logger.LogDebug("Loaded camera list from cache.");
                return resp;
            }

            var query = await GetUrl(StationConstants.Api.ApiCamera.Methods.List, version: 9, parameter: new System.Collections.Generic.Dictionary<string, object>()
            {
                { "limit", 100 }
            }, cancellation: cancellation);

            var response = await Client.CallGetApiAsync<ApiCameraListResponse>(string.Empty, query, token: cancellation);

            if (response?.Success == true)
            {
                Logger.LogDebug("Retrieved camera list form station.");
                Cache.Set<ApiCameraListResponse>(Constants.Cache.SettingKeys.StationCameraListCache, CameraListKey, response);
                return response;
            }
            else
            {
                Logger.LogDebug("Error loading camera list form station with code '{errorCode}'.", response.Error?.Code ?? -1);
                // trhow error
            }

            return response;
        }
    }
}
