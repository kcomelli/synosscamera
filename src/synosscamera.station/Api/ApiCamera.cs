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

            if (await VerifyLoggedIn(cancellation))
            {

                var query = await GetUrl(StationConstants.Api.ApiCamera.Methods.List, version: 9, parameter: new System.Collections.Generic.Dictionary<string, object>()
                {
                    { "limit", 100 }
                }, cancellation: cancellation);

                var response = await Client.CallGetApiAsync<ApiCameraListResponse>(query.action, query.query, token: cancellation);

                if (response?.Success == true)
                {
                    Logger.LogDebug("Retrieved camera list form station.");
                    Cache.Set<ApiCameraListResponse>(Constants.Cache.SettingKeys.StationCameraListCache, CameraListKey, response);
                    return response;
                }
                else
                {
                    Logger.LogDebug("Error loading camera list form station with code '{errorCode}'.", response.Error?.Code ?? -1);
                    var ex = Client.LastError as StationApiException;
                    if (ex == null)
                        ex = new StationApiException("Error loading camera list from station");

                    var errorInfo = ErrorResponseFromStationError(response.Error);
                    ex.ErrorResponse = errorInfo.error;
                    ex.UpdateStatusCode(errorInfo.statusCode);

                    throw ex;
                }
            }

            return null;
        }

        /// <summary>
        /// Get camera infos from station
        /// </summary>
        /// <param name="cameraId">Camer id</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Api response from station</returns>
        public async Task<ApiCameraGetInfoResponse> GetCameraInfoAsync(int cameraId, CancellationToken cancellation = default)
        {

            if (await VerifyLoggedIn(cancellation))
            {

                var query = await GetUrl(StationConstants.Api.ApiCamera.Methods.GetInfo, version: 8, parameter: new System.Collections.Generic.Dictionary<string, object>()
                {
                    { "basic", true },
                    { "cameraIds", cameraId },
                    { "optimize", true }
                }, cancellation: cancellation);

                var response = await Client.CallGetApiAsync<ApiCameraGetInfoResponse>(query.action, query.query, token: cancellation);

                if (response?.Success == true)
                {
                    Logger.LogDebug("Retrieved camera info form station.");
                    return response;
                }
                else
                {
                    Logger.LogDebug("Error loading camera info form station with code '{errorCode}'.", response.Error?.Code ?? -1);
                    var ex = Client.LastError as StationApiException;
                    if (ex == null)
                        ex = new StationApiException("Error loading camera info from station");

                    var errorInfo = ErrorResponseFromStationError(response.Error);
                    ex.ErrorResponse = errorInfo.error;
                    ex.UpdateStatusCode(errorInfo.statusCode);

                    throw ex;
                }
            }

            return null;
        }
    }
}
