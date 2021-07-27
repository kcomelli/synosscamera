using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using synosscamera.core;
using synosscamera.core.Diagnostics;
using synosscamera.core.Infrastructure.Http;
using synosscamera.core.Model.Dto;
using synosscamera.core.Model.Dto.Camera;
using synosscamera.station.Api;
using synosscamera.station.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace synosscamera.api.Controllers
{
    /// <summary>
    /// Camera controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = Constants.Security.ApiKeyAuthenticationScheme)]
    public class CameraController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ApiCamera _apiCamera;
        private readonly ApiExternalRecording _apiRecording;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="cameraApi">Camnera API</param>
        /// <param name="recordingApi"></param>
        /// <param name="logger">Logger</param>
        /// <param name="mapper"></param>
        public CameraController(ApiCamera cameraApi, ApiExternalRecording recordingApi, ILogger<CameraController> logger, IMapper mapper)
        {
            logger.CheckArgumentNull(nameof(logger));
            cameraApi.CheckArgumentNull(nameof(cameraApi));
            recordingApi.CheckArgumentNull(nameof(recordingApi));
            mapper.CheckArgumentNull(nameof(mapper));

            _logger = logger;
            _apiCamera = cameraApi;
            _mapper = mapper;
            _apiRecording = recordingApi;
        }

        /// <summary>
        /// Get a list of installed cameras of the connected surveillance station
        /// </summary>
        /// <remarks> 
        /// <para>
        /// <code>GET api/camera</code>
        /// </para>
        /// <para>
        /// Your connection to surveillance station must be completed at server level in order to connect sucessfully!
        /// </para>
        /// </remarks>
        /// <returns>If successful, returns a list of installed cameras.</returns>
        /// <response code="200">Response object containing a list of installed cameras and their information.</response>    
        /// <response code="401">Authentication is required in order to query the camera list.</response>
        /// <response code="403">The current authenticated client or user does not have permissions to query the list of cameras.</response>
        /// <response code="408">The connection to surveillance station or a camera timed out. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="429">Max tries reached at Surveillance API level. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="500">If a general error occured. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="501">A called API, used method or version is not available at the SurveillanceStation. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="503">May occure if a limitation is reached. Retry the request later. An ApiErrorResponse result will be sent within the response body.</response>
        [HttpGet]
        [ProducesResponseType(typeof(CameraListResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 408)]
        [ProducesResponseType(typeof(ApiErrorResponse), 429)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        [ProducesResponseType(typeof(ApiErrorResponse), 501)]
        [ProducesResponseType(typeof(ApiErrorResponse), 503)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var cameras = await _apiCamera.GetCameraListAsync(HttpContext.RequestAborted);

                var ret = new CameraListResponse();
                ret.Cameras = _mapper.Map<List<CameraDetails>>(cameras.Data.Cameras);

                return Ok(ret);
            }
            catch (StationApiException sex)
            {
                return StatusCode((int)sex.ResponseStatus, sex.ErrorResponse);
            }
            catch (ApiClientException sex)
            {
                return StatusCode((int)sex.ResponseStatus, sex.ErrorResponse);
            }
        }

        /// <summary>
        /// Get a special info and state of a camera
        /// </summary>
        /// <remarks> 
        /// <para>
        /// <code>GET api/camera/{id}</code>
        /// </para>
        /// <para>
        /// Your connection to surveillance station must be completed at server level in order to connect sucessfully!
        /// </para>
        /// </remarks>
        /// <param name="id">Id of the camera to get special info and state.</param>
        /// <returns>If successful, returns the information of the requested camera.</returns>
        /// <response code="200">Response object containing the info and state of the camera..</response>    
        /// <response code="401">Authentication is required in order to query the camera info.</response>
        /// <response code="403">The current authenticated client or user does not have permissions to query the info of camera.</response>
        /// <response code="404">The requested camera id was not found. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="408">The connection to surveillance station or a camera timed out. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="429">Max tries reached at Surveillance API level. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="500">If a general error occured. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="501">A called API, used method or version is not available at the SurveillanceStation. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="503">May occure if a limitation is reached. Retry the request later. An ApiErrorResponse result will be sent within the response body.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CameraInfoResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 408)]
        [ProducesResponseType(typeof(ApiErrorResponse), 429)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        [ProducesResponseType(typeof(ApiErrorResponse), 501)]
        [ProducesResponseType(typeof(ApiErrorResponse), 503)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var cameras = await _apiCamera.GetCameraInfoAsync(id, HttpContext.RequestAborted);

                var ret = new CameraInfoResponse();
                if (cameras.Data.Cameras.Any())
                    ret.Data = _mapper.Map<CameraDetailsSpecialized>(cameras.Data.Cameras.First());
                else
                    return NotFound(new ApiErrorResponse()
                    {
                        IsApiError = true,
                        Errors = new ApiError[]
                        {
                            new ApiError()
                            {
                                ErrorCode = "404",
                                ErrorMessage = $"Camera with id '{id}' could not be found!",
                                ErrorType = ApiErrorTypes.ExternalRestApi
                            }
                        }
                    });

                return Ok(ret);
            }
            catch (StationApiException sex)
            {
                return StatusCode((int)sex.ResponseStatus, sex.ErrorResponse);
            }
            catch (ApiClientException sex)
            {
                return StatusCode((int)sex.ResponseStatus, sex.ErrorResponse);
            }
        }


        /// <summary>
        /// Start the external recording of the given camera id
        /// </summary>
        /// <remarks> 
        /// <para>
        /// <code>GET api/camera/{id}/recording/start</code>
        /// </para>
        /// <para>
        /// Your connection to surveillance station must be completed at server level in order to connect sucessfully!<br/>
        /// You can use the <code>api/camera/{id}</code> endpoint to get current state of recording!
        /// </para>
        /// </remarks>
        /// <param name="id">Id of the camera which should start recording.</param>
        /// <returns>If successful, returns the information of execution success.</returns>
        /// <response code="200">Response object containing the state of execution.</response>    
        /// <response code="401">Authentication is required in order to start recording.</response>
        /// <response code="403">The current authenticated client or user does not have permissions to start recording.</response>
        /// <response code="408">The connection to surveillance station or a camera timed out. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="429">Max tries reached at Surveillance API level. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="500">If a general error occured. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="501">A called API, used method or version is not available at the SurveillanceStation. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="503">May occure if a limitation is reached. Retry the request later. An ApiErrorResponse result will be sent within the response body.</response>
        [HttpPost("{id}/recording/start")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 408)]
        [ProducesResponseType(typeof(ApiErrorResponse), 429)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        [ProducesResponseType(typeof(ApiErrorResponse), 501)]
        [ProducesResponseType(typeof(ApiErrorResponse), 503)]
        public async Task<IActionResult> StartRecording(int id)
        {
            try
            {
                var resp = await _apiRecording.StartRecordingAsync(id, HttpContext.RequestAborted);

                var ret = new SuccessResponse();
                ret.Success = resp.Success;

                return Ok(ret);
            }
            catch (StationApiException sex)
            {
                return StatusCode((int)sex.ResponseStatus, sex.ErrorResponse);
            }
            catch (ApiClientException sex)
            {
                return StatusCode((int)sex.ResponseStatus, sex.ErrorResponse);
            }
        }

        /// <summary>
        /// Stop the external recording of the given camera id
        /// </summary>
        /// <remarks> 
        /// <para>
        /// <code>GET api/camera/{id}/recording/stop</code>
        /// </para>
        /// <para>
        /// Your connection to surveillance station must be completed at server level in order to connect sucessfully!<br/>
        /// You can use the <code>api/camera/{id}</code> endpoint to get current state of recording!
        /// </para>
        /// </remarks>
        /// <param name="id">Id of the camera which should stop recording.</param>
        /// <returns>If successful, returns the information of execution succeess.</returns>
        /// <response code="200">Response object containing the state of execution.</response>    
        /// <response code="401">Authentication is required in order to stop recording.</response>
        /// <response code="403">The current authenticated client or user does not have permissions to stop recording.</response>
        /// <response code="408">The connection to surveillance station or a camera timed out. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="429">Max tries reached at Surveillance API level. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="500">If a general error occured. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="501">A called API, used method or version is not available at the SurveillanceStation. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="503">May occure if a limitation is reached. Retry the request later. An ApiErrorResponse result will be sent within the response body.</response>
        [HttpPost("{id}/recording/stop")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 408)]
        [ProducesResponseType(typeof(ApiErrorResponse), 429)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        [ProducesResponseType(typeof(ApiErrorResponse), 501)]
        [ProducesResponseType(typeof(ApiErrorResponse), 503)]
        public async Task<IActionResult> StopRecording(int id)
        {
            try
            {
                var resp = await _apiRecording.StopRecordingAsync(id, HttpContext.RequestAborted);

                var ret = new SuccessResponse();
                ret.Success = resp.Success;

                return Ok(ret);
            }
            catch (StationApiException sex)
            {
                return StatusCode((int)sex.ResponseStatus, sex.ErrorResponse);
            }
            catch (ApiClientException sex)
            {
                return StatusCode((int)sex.ResponseStatus, sex.ErrorResponse);
            }
        }
    }
}
