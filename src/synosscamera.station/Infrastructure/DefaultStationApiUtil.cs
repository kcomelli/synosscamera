using Microsoft.Extensions.Logging;
using synosscamera.core.Diagnostics;
using synosscamera.station.Abstractions;
using synosscamera.station.Api;
using synosscamera.station.Model.ApiInfo;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Infrastructure
{
    /// <summary>
    /// Station API utility
    /// </summary>
    public class DefaultStationApiUtil : IStationApiUtil
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IStationApi> _stationApis;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="stationApis">Registered station API classes</param>
        public DefaultStationApiUtil(ILogger<DefaultStationApiUtil> logger, IEnumerable<IStationApi> stationApis)
        {
            logger.CheckArgumentNull(nameof(logger));
            stationApis.CheckArgumentNull(nameof(stationApis));

            _logger = logger;
            _stationApis = stationApis;
        }
        /// <summary>
        /// Get the registered ApiInfo class
        /// </summary>
        protected ApiInfo ApiInfo => (ApiInfo)_stationApis.FirstOrDefault(o => o is ApiInfo);
        /// <summary>
        /// Get the registered ApiAuth class
        /// </summary>
        protected ApiAuth ApiAuth => (ApiAuth)_stationApis.FirstOrDefault(o => o is ApiAuth);

        /// <inheritdoc/>
        public async Task<Dictionary<string, ApiDetails>> ApiList(CancellationToken cancellationToken = default)
        {
            if (ApiInfo != null)
            {
                var response = await ApiInfo.GetApisAsync(cancellation: cancellationToken);
                if (response.Success)
                {
                    if (_stationApis != null)
                    {
                        foreach (var api in _stationApis)
                        {
                            api.SetApiList(response.Data);
                        }
                    }
                    return response.Data;
                }
            }

            return new Dictionary<string, ApiDetails>();
        }

        /// <inheritdoc/>
        public async Task<bool> Login(CancellationToken cancellationToken = default)
        {
            if (ApiAuth != null)
            {
                var response = await ApiAuth.LoginAsync(cancellationToken);

                if (response.Success)
                {
                    if (_stationApis != null)
                    {
                        foreach (var api in _stationApis)
                        {
                            api.SynoToken = response.Data.Synotoken;
                        }
                    }
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> Logout(CancellationToken cancellationToken = default)
        {
            if (ApiAuth != null)
            {
                var response = await ApiAuth.LogoutAsync(cancellationToken);

                if (response.Success)
                {
                    if (_stationApis != null)
                    {
                        foreach (var api in _stationApis)
                        {
                            api.SynoToken = null;
                        }
                    }
                    return true;
                }
            }

            return false;
        }
    }
}
