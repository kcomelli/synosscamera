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
        public override string ApiName => StationConstants.Api.ApiAuth.Name;

        /// <inheritdoc/>
        protected override void BuildApiErrorCodeMappings()
        {
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.AccountNotSpecified] = "The account parameter is not specified.";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.InvalidPassword] = "Invalid password.";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.GuestOrDisabledAccount] = "Guest or disabled account.";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.PermissionDeined] = "Permission denied.";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.OneTimePasswordNotSpecified] = "One time password not specified.";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.OneTimePasswordAuthFailed] = "One time password authenticate failed.";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.AppPortalIncorrect] = "App portal incorrect.";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.OTPCodeEnforced] = "OTP code enforced.";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.MaxTriesReached] = "Max Tries (if auto blocking is set to true).";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.PasswordExpiredCannotChange] = "Password Expired Can not Change.";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.PasswordExpired] = "Password Expired.";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.PasswordMustBeChanged] = "Account Locked(when account max try exceed).";
            ErrorCodeMappings[StationConstants.Api.ApiAuth.ErrorCodes.AccountLocked] = "The account parameter is not specified.";
        }

        /// <summary>
        /// Login to station using configured credentials
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<ApiAuthLoginResponse> LoginAsync(CancellationToken cancellationToken = default)
        {
            var query = await GetUrl(StationConstants.Api.ApiAuth.Methods.Login, version: 3, parameter: new System.Collections.Generic.Dictionary<string, object>()
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
            var query = await GetUrl(StationConstants.Api.ApiAuth.Methods.Logout, version: 3, parameter: new System.Collections.Generic.Dictionary<string, object>()
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
