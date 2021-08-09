using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using synosscamera.core.Diagnostics;
using synosscamera.station.Abstractions;
using synosscamera.station.Api;
using synosscamera.station.Model.ApiInfo;
using System;
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
        private IServiceProvider _serviceProvider;
        private IEnumerable<IStationApi> _stationApis = null;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="sp">Service provider</param>
        public DefaultStationApiUtil(ILogger<DefaultStationApiUtil> logger, IServiceProvider sp)
        {
            logger.CheckArgumentNull(nameof(logger));
            sp.CheckArgumentNull(nameof(sp));

            _logger = logger;
            _serviceProvider = sp;


        }
        /// <summary>
        /// Get the list of registered station APIs
        /// </summary>
        protected IEnumerable<IStationApi> StationApis
        {
            get
            {
                if (_stationApis == null)
                    _stationApis = _serviceProvider.GetServices<IStationApi>();

                return _stationApis;
            }
        }
        /// <summary>
        /// Get the registered ApiInfo class
        /// </summary>
        protected ApiInfo ApiInfo => (ApiInfo)StationApis.FirstOrDefault(o => o is ApiInfo);
        /// <summary>
        /// Get the registered ApiAuth class
        /// </summary>
        protected ApiAuth ApiAuth => (ApiAuth)StationApis.FirstOrDefault(o => o is ApiAuth);

        /// <inheritdoc/>
        public async Task<Dictionary<string, ApiDetails>> ApiList(CancellationToken cancellationToken = default)
        {
            if (ApiInfo != null)
            {
                var response = await ApiInfo.GetApisAsync(cancellation: cancellationToken);
                if (response.Success)
                {
                    if (StationApis != null)
                    {
                        foreach (var api in StationApis)
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
                    if (StationApis != null)
                    {
                        foreach (var api in StationApis)
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
                    if (StationApis != null)
                    {
                        foreach (var api in StationApis)
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
