using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core.Abstractions;
using synosscamera.station.Abstractions;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using synosscamera.station.Model;
using synosscamera.station.Model.ApiInfo;
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

                var query = await GetUrl(StationConstants.Api.ApiHomeMode.Methods.Switch, version: 1, parameter: new System.Collections.Generic.Dictionary<string, object>()
                {
                    { "on", on.ToString().ToLower() }
                }, cancellation: cancellation);

                var response = await Client.CallGetApiAsync<ApiCameraListResponse>(query.action, query.query, token: cancellation);

                if (response?.Success == true)
                {
                    Logger.LogDebug("Retrieved home mode switched response.");
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
    }
}
