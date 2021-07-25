using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core.Abstractions;
using synosscamera.core.Diagnostics;
using synosscamera.core.Extensions;
using synosscamera.station.Abstractions;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Api
{
    /// <summary>
    /// Api base class for APIs which require auth and need a login-logout
    /// </summary>
    public abstract class AuthenticatedStationApiBase : StationApiBase
    {
        private readonly IStationApiUtil _apiUtil;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public AuthenticatedStationApiBase(SurveillanceStationClient client, IOptions<SurveillanceStationSettings> settings, IMemoryCacheWrapper memoryCache, ILoggerFactory loggerFactory,
            IStationApiUtil apiUtil)
            : base(client, settings, memoryCache, loggerFactory)
        {
            apiUtil.CheckArgumentNull(nameof(apiUtil));

            _apiUtil = apiUtil;
        }
        /// <summary>
        /// Access station API util
        /// </summary>
        protected IStationApiUtil StationApiUtil => _apiUtil;

        /// <summary>
        /// Verify that the user is logged in
        /// </summary>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>True if login suceed or already logged in</returns>
        protected async Task<bool> VerifyLoggedIn(CancellationToken cancellation = default)
        {
            // load and cache api list
            ((IStationApi)this).SetApiList(await StationApiUtil.ApiList(cancellation));

            if (SynoToken.IsMissing())
            {
                return await StationApiUtil.Login(cancellation);
            }

            return true;
        }
    }
}
