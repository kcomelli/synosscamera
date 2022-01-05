using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using synosscamera.core;
using synosscamera.core.Diagnostics;
using synosscamera.core.Model.Dto;
using synosscamera.station.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace synosscamera.api.Controllers
{
    /// <summary>
    /// Info controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = Constants.Security.ApiKeyAuthenticationScheme)]
    public class InfoController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly SurveillanceStationSettings _settings;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="env"></param>
        /// <param name="settings"></param>
        public InfoController(IWebHostEnvironment env, IOptions<SurveillanceStationSettings> settings)
        {
            env.CheckArgumentNull(nameof(env));
            _env = env;
            _settings = settings?.Value ?? new SurveillanceStationSettings();
        }

        /// <summary>
        /// Get api information.
        /// </summary>
        /// <remarks> 
        /// </remarks>
        /// <returns>If successful, returns an information object.</returns>
        /// <response code="200">Response object containing api information.</response>    
        /// <response code="401">Authentication is required in order to query the api information.</response>
        /// <response code="403">The current authenticated client or user does not have permissions to query the the api information.</response>
        /// <response code="408">The connection to surveillance station or a camera timed out. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="429">Max tries reached at Surveillance API level. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="500">If a general error occured. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="501">A called API, used method or version is not available at the SurveillanceStation. An ApiErrorResponse result will be sent within the response body.</response>
        /// <response code="503">May occure if a limitation is reached. Retry the request later. An ApiErrorResponse result will be sent within the response body.</response>
        [HttpGet]
        [ProducesResponseType(typeof(InfoResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 408)]
        [ProducesResponseType(typeof(ApiErrorResponse), 429)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        [ProducesResponseType(typeof(ApiErrorResponse), 501)]
        [ProducesResponseType(typeof(ApiErrorResponse), 503)]
        public IActionResult Get()
        {
            var ret = new InfoResponse()
            {
                Version = Assembly.GetEntryAssembly().GetName().Version.ToString(),
                WebRootPath = _env.WebRootPath,
                ContentPath = _env.ContentRootPath,
                StationEndpoint = _settings.BaseUrl
            };

            return Ok(ret);
        }
    }
}
