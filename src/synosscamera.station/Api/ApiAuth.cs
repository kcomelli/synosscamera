using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core.Abstractions;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using synosscamera.station.Model.ApiInfo;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Api
{
    /// <summary>
    /// Perform login and logout
    /// </summary>
    public class ApiAuth : StationApiBase
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public ApiAuth(SurveillanceStationClient client, IOptions<SurveillanceStationSettings> settings, IMemoryCacheWrapper memoryCache, ILoggerFactory loggerFactory)
            : base(client, settings, memoryCache, loggerFactory)
        {
        }

        /// <summary>
        /// Api name
        /// </summary>
        public override string ApiName => "SYNO.API.Auth";

        /// <summary>
        /// Login to station using configured credentials
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<ApiAuthLoginResponse> LoginAsync(CancellationToken cancellationToken = default)
        {
            var query = await GetUrl("login", version: 3, parameter: new System.Collections.Generic.Dictionary<string, object>()
            {
                { "account",  Settings.Username },
                { "passwd",  Settings.Password },
                { "format",  "sid" },
                { "session",  Settings.SessionNameForStation },
                { "enable_syno_token",  "yes" }
            }, cancellation: cancellationToken);

            var response = await Client.CallGetApiAsync<ApiAuthLoginResponse>(string.Empty, query, token: cancellationToken);

            if (response?.Success == true)
            {
                Logger.LogDebug("Successfully logged in to station.");
                Client.SynoToken = response.Data.Synotoken;
                return response;
            }
            else
            {
                Logger.LogDebug("Error logging into station with code '{errorCode}'.", response.Error?.Code ?? -1);
                // trhow error

            }

            return response;
        }

        /// <summary>
        /// Login to station using configured credentials
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<ApiAuthLogoutResponse> LogoutAsync(CancellationToken cancellationToken = default)
        {
            var query = await GetUrl("logout", version: 3, parameter: new System.Collections.Generic.Dictionary<string, object>()
            {
                { "session",  Settings.SessionNameForStation }
            }, cancellation: cancellationToken);

            var response = await Client.CallGetApiAsync<ApiAuthLogoutResponse>(string.Empty, query, token: cancellationToken);

            if (response?.Success == true)
            {
                Logger.LogDebug("Successfully logged out of station.");
                Client.SynoToken = null;
                return response;
            }
            else
            {
                Logger.LogDebug("Error logging out of station with code '{errorCode}'.", response.Error?.Code ?? -1);
                // trhow error

            }

            return response;
        }

        /// <inheritdoc/>
        protected override Task<string> ResolvePathForApi(CancellationToken cancellation = default)
        {
            return Task.FromResult("auth.cgi");
        }
        /// <inheritdoc/>
        protected override Task<(int minVersion, int maxVersion)> ResolveVersionsForApi(CancellationToken cancellation = default)
        {
            return Task.FromResult((1, 3));
        }
    }
}
