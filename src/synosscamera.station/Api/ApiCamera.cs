using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using synosscamera.core;
using synosscamera.core.Abstractions;
using synosscamera.core.Extensions;
using synosscamera.station.Abstractions;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using synosscamera.station.Internals;
using synosscamera.station.Model.ApiInfo;
using System;
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
                    Logger.LogDebug("Error loading camera list form station with code '{errorCode}'.", response?.Error?.Code ?? -1);
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
                    { "optimize", true },
                    { "camAppInfo", true },
                }, cancellation: cancellation);

                var response = await Client.CallGetApiAsync<ApiCameraGetInfoResponse>(query.action, query.query, token: cancellation);

                if (response?.Success == true)
                {
                    Logger.LogDebug("Retrieved camera info form station.");
                    return response;
                }
                else
                {
                    Logger.LogDebug("Error loading camera info form station with code '{errorCode}'.", response?.Error?.Code ?? -1);
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

        /// <summary>
        /// Update the schedule
        /// </summary>
        /// <param name="cameraId"></param>
        /// <param name="schedule"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<ApiCameraSaveResponse> ChangeSchedule(int cameraId, RecordSchedule schedule, DateTime? from = null, DateTime? to = null, CancellationToken cancellation = default)
        {
            if (await VerifyLoggedIn(cancellation))
            {
                #region Tests
                //var query = await GetFormData(StationConstants.Api.ApiCamera.Methods.Save, version: 9, parameter: new System.Collections.Generic.Dictionary<string, object>()
                //{
                //    { "camId", cameraId },
                //    { "id", cameraId },
                //    { "data", new System.Collections.Generic.Dictionary<string, object>() { { "recordSchedule", ApiUtilities.RecordingScheduleToString(ApiUtilities.CreateRecordingSchedule(schedule, from, to, null)) }}
                //} });

                //var data = new System.Collections.Generic.Dictionary<string, object>()
                //{
                //    { "camId", cameraId },
                //    { "api", "SYNO.SurveillanceStation.Camera.Wizard" },
                //    { "method", "CamSaveAll" },
                //    { "version", 2 },
                //    { "actFormHost", false },
                //    { "data", JsonConvert.SerializeObject(new System.Collections.Generic.Dictionary<string, object>()
                //                                            {
                //                                                { "camId", cameraId },
                //                                                { "recordSchedule", ApiUtilities.RecordingScheduleToString(ApiUtilities.CreateRecordingSchedule(schedule, from, to, null)) }
                //                                            })
                //    }
                //};

                //var response = await Client.CallPostApiAsync<ApiCameraSaveResponse>(query.action, data, token: cancellation);
                #endregion

                var query = await GetFormData(StationConstants.Api.ApiCamera.Methods.Save, version: 9, parameter: new System.Collections.Generic.Dictionary<string, object>()
                {
                    { "id", cameraId },
                    // sending a string without a character don't work - don't know why
                    // maybe bug in synos API
                    { "recordSchedule", ApiUtilities.RecordingScheduleToString(ApiUtilities.CreateRecordingSchedule(schedule, from, to, null)).Insert(9,",") }
                }, cancellation: cancellation);

                var response = await Client.CallPostApiAsync<ApiCameraSaveResponse>(query.action, query.formdata, token: cancellation);

                if (response?.Success == true)
                {
                    Logger.LogDebug("Sucessfully changed schedule.");
                    return response;
                }
                else
                {
                    Logger.LogDebug("Error changing recording schedule of station camera with code '{errorCode}'.", response?.Error?.Code ?? -1);
                    var ex = Client.LastError as StationApiException;
                    if (ex == null)
                        ex = new StationApiException("Error changing recording schedule of station camera");

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
