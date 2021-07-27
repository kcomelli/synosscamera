using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core.Abstractions;
using synosscamera.station.Abstractions;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using synosscamera.station.Model;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Api
{
    /// <summary>
    /// Api camera external recording
    /// </summary>
    public class ApiExternalRecording : AuthenticatedStationApiBase
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public ApiExternalRecording(SurveillanceStationClient client, IOptions<SurveillanceStationSettings> settings, IMemoryCacheWrapper memoryCache, ILoggerFactory loggerFactory, IStationApiUtil apiUtil)
            : base(client, settings, memoryCache, loggerFactory, apiUtil)
        {
        }

        /// <summary>
        /// Api name
        /// </summary>
        public override string ApiName => StationConstants.Api.ApiExternalRecording.Name;

        /// <inheritdoc/>
        protected override void BuildApiErrorCodeMappings()
        {
            ErrorCodeMappings[StationConstants.Api.ApiExternalRecording.ErrorCodes.ExecutionFailed] = "Execution failed.";
            ErrorCodeMappings[StationConstants.Api.ApiExternalRecording.ErrorCodes.ParameterInvalid] = "Parameter invalid.";
            ErrorCodeMappings[StationConstants.Api.ApiExternalRecording.ErrorCodes.CameraDisabled] = "The camera is disabled.";
        }

        /// <summary>
        /// Start external recording for camera with given id
        /// </summary>
        /// <param name="cameraId">Id of the camera to perform this action on</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Api response from station</returns>
        public async Task<StationResponseBase> StartRecordingAsync(int cameraId, CancellationToken cancellation = default)
        {
            if (await VerifyLoggedIn(cancellation))
            {

                var query = await GetUrl(StationConstants.Api.ApiExternalRecording.Methods.Record,
                    StationConstants.Api.ApiExternalRecording.Actions.Start, version: 1, parameter: new System.Collections.Generic.Dictionary<string, object>()
                {
                    { "cameraId", cameraId }
                }, cancellation: cancellation);

                var response = await Client.CallGetApiAsync<StationResponseBase>(query.action, query.query, token: cancellation);

                if (response?.Success == true)
                {
                    Logger.LogDebug("Started external recording of camera '{cameraId}' on station.", cameraId);
                    return response;
                }
                else
                {
                    Logger.LogDebug("Error starting external recording of camera '{cameraId}' on station with code '{errorCode}'.", cameraId, response.Error?.Code ?? -1);
                    var ex = Client.LastError as StationApiException;
                    if (ex == null)
                        ex = new StationApiException($"Error starting external record of camera '{cameraId}'.");

                    var errorInfo = ErrorResponseFromStationError(response.Error);
                    ex.ErrorResponse = errorInfo.error;
                    ex.UpdateStatusCode(errorInfo.statusCode);

                    throw ex;
                }
            }

            return null;
        }

        /// <summary>
        /// Stop external recording for camera with given id
        /// </summary>
        /// <param name="cameraId">Id of the camera to perform this action on</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Api response from station</returns>
        public async Task<StationResponseBase> StopRecordingAsync(int cameraId, CancellationToken cancellation = default)
        {
            if (await VerifyLoggedIn(cancellation))
            {

                var query = await GetUrl(StationConstants.Api.ApiExternalRecording.Methods.Record,
                    StationConstants.Api.ApiExternalRecording.Actions.Stop, version: 1, parameter: new System.Collections.Generic.Dictionary<string, object>()
                {
                    { "cameraId", cameraId }
                }, cancellation: cancellation);

                var response = await Client.CallGetApiAsync<StationResponseBase>(query.action, query.query, token: cancellation);

                if (response?.Success == true)
                {
                    Logger.LogDebug("Stopped external recording of camera '{cameraId}' on station.", cameraId);
                    return response;
                }
                else
                {
                    Logger.LogDebug("Error stopping external recording of camera '{cameraId}' on station with code '{errorCode}'.", cameraId, response.Error?.Code ?? -1);
                    var ex = Client.LastError as StationApiException;
                    if (ex == null)
                        ex = new StationApiException($"Error stopping external record of camera '{cameraId}'.");

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
