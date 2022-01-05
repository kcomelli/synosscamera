using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core;
using synosscamera.core.Abstractions;
using synosscamera.station.Abstractions;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using synosscamera.station.Model;
using synosscamera.station.Model.ApiInfo;
using synosscamera.core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Api
{
    /// <summary>
    /// Api home mode
    /// </summary>
    public class ApiHomeMode : AuthenticatedStationApiBase
    {
        /// <summary>
        /// HomeMode cache key
        /// </summary>
        internal const string HomeModeCacheKey = "homemode";
        
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        /// <param name="memoryCache"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="apiUtil"></param>
        public ApiHomeMode(SurveillanceStationClient client, IOptions<SurveillanceStationSettings> settings, IMemoryCacheWrapper memoryCache, ILoggerFactory loggerFactory, IStationApiUtil apiUtil)
            : base(client, settings, memoryCache, loggerFactory, apiUtil)
        {
        }

        /// <summary>
        /// Api name
        /// </summary>
        public override string ApiName => StationConstants.Api.ApiHomeMode.Name;

        /// <inheritdoc/>
        protected override void BuildApiErrorCodeMappings()
        {
        }

        /// <summary>
        /// Switch Home mode
        /// </summary>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Api response from station</returns>
        public async Task<StationResponseBase> SwitchHomeMode(bool on, CancellationToken cancellation = default)
        {
            if (await VerifyLoggedIn(cancellation))
            {
                Logger.LogDebug("Switching home mode at station to '{on}'.", on);
                var query = await GetUrl(StationConstants.Api.ApiHomeMode.Methods.Switch, version: 1, parameter: new System.Collections.Generic.Dictionary<string, object>()
                {
                    { "on", on.ToString().ToLower() }
                }, cancellation: cancellation);

                var response = await Client.CallGetApiAsync<ApiCameraListResponse>(query.action, query.query, token: cancellation);

                if (response?.Success == true)
                {
                    Logger.LogDebug("Retrieved home mode switched response. Loading home mode info from cache or api.");
                    var currentModeResponse = await GetInfo(cancellation);
                    if(currentModeResponse?.Data != null && currentModeResponse.Data.On != on)
                    {
                        // update cache
                        currentModeResponse.Data.On = on;
                        Cache.Set<HomeModeInfoResponse>(Constants.Cache.SettingKeys.HomeModeCache, HomeModeCacheKey, currentModeResponse);
                        Logger.LogDebug("Updateing home mode cache.");
                        return currentModeResponse;
                    }
                    return response;
                }
                else
                {
                    Logger.LogDebug("Error switching home mode with code '{errorCode}'.", response?.Error?.Code ?? -1);

                    
                    var ex = Client.LastError as StationApiException;
                    if (ex == null)
                        ex = new StationApiException("Error switching home mode");

                    var errorInfo = ErrorResponseFromStationError(response?.Error);
                    ex.ErrorResponse = errorInfo.error;
                    ex.UpdateStatusCode(errorInfo.statusCode);

                    throw ex;
                }
            }

            return null;
        }

        /// <summary>
        /// Get home mode info
        /// </summary>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Api response from station</returns>
        public async Task<HomeModeInfoResponse> GetInfo(CancellationToken cancellation = default)
        {
            if (Cache.TryGetValue<HomeModeInfoResponse>(Constants.Cache.SettingKeys.HomeModeCache, HomeModeCacheKey, out HomeModeInfoResponse resp))
            {
                Logger.LogDebug("Loaded home mode from cache.");
                return resp;
            }

            if (await VerifyLoggedIn(cancellation))
            {
                Logger.LogDebug("Querying home mode from station.");
                var query = await GetUrl(StationConstants.Api.ApiHomeMode.Methods.GetInfo, version: 1, cancellation: cancellation);

                var response = await Client.CallGetApiAsync<HomeModeInfoResponse>(query.action, query.query, token: cancellation);

                if (response?.Success == true)
                {
                    Logger.LogDebug("Retrieved home mode info response.");
                    Cache.Set<HomeModeInfoResponse>(Constants.Cache.SettingKeys.HomeModeCache, HomeModeCacheKey, response);
                    return response;
                }
                else
                {
                    Logger.LogDebug("Error reading home mode info with code '{errorCode}'.", response?.Error?.Code ?? -1);
                    var ex = Client.LastError as StationApiException;
                    if (ex == null)
                        ex = new StationApiException("Error reading home mode info");

                    var errorInfo = ErrorResponseFromStationError(response?.Error);
                    ex.ErrorResponse = errorInfo.error;
                    ex.UpdateStatusCode(errorInfo.statusCode);

                    throw ex;
                }
            }

            return null;
        }
    }
}
