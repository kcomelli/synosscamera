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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace synosscamera.api.Controllers
{
    /// <summary>
    /// Station controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = Constants.Security.ApiKeyAuthenticationScheme)]
    public class StationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ApiHomeMode _apiHomeMode;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="apiHomeMode"></param>
        /// <param name="logger">Logger</param>
        /// <param name="mapper"></param>
        public StationController(ApiHomeMode apiHomeMode, ILogger<StationController> logger, IMapper mapper)
        {
            logger.CheckArgumentNull(nameof(logger));
            apiHomeMode.CheckArgumentNull(nameof(apiHomeMode));
            mapper.CheckArgumentNull(nameof(mapper));

            _logger = logger;
            _mapper = mapper;
            _apiHomeMode = apiHomeMode;
        }

        /// <summary>
        /// Activate or deactivate the Home-Mode
        /// </summary>
        /// <remarks> 
        /// <para>
        /// <code>POST api/homemode?on=true</code>
        /// </para>
        /// <para>
        /// Your connection to surveillance station must be completed at server level in order to connect sucessfully!<br/>
        /// You can use the <code>api/camera/{id}</code> endpoint to get current state of recording!
        /// </para>
        /// </remarks>
        /// <param name="on">If true, Home-Mode will be switched on.</param>
        /// <returns>If successful, returns the information of execution success.</returns>
        /// <response code="200">Response object containing the state of execution.</response>    
        /// <response code="401">Authentication is required in order to set the home mode.</response>
        /// <response code="403">The current authenticated client or user does not have permissions to set the home mode.</response>
        /// <response code="408">The connection to surveillance station or a camera timed out. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="429">Max tries reached at Surveillance API level. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="500">If a general error occured. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="501">A called API, used method or version is not available at the SurveillanceStation. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="503">May occure if a limitation is reached. Retry the request later. An ApiErrorResponse result will be sent within the response body.</response>
        [HttpPost("/homemode")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 408)]
        [ProducesResponseType(typeof(ApiErrorResponse), 429)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        [ProducesResponseType(typeof(ApiErrorResponse), 501)]
        [ProducesResponseType(typeof(ApiErrorResponse), 503)]
        public async Task<IActionResult> HomeMode([FromQuery][Required] bool on)
        {
            try
            {
                var resp = await _apiHomeMode.SwitchHomeMode(on, HttpContext.RequestAborted);

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
