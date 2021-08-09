using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.core.Abstractions;
using synosscamera.core.Extensions;
using synosscamera.core.Infrastructure.Http;
using synosscamera.station.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Infrastructure
{
    /// <summary>
    /// Http client for surveillance station web api
    /// </summary>
    public class SurveillanceStationClient : RestApiClientBase
    {
        private readonly SurveillanceStationSettings _settings;
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="settings">Sateion settings</param>
        /// <param name="clientFactory"></param>
        /// <param name="loggerFactory">Logger factory for creating loggers</param>
        /// <param name="cache">Local memory cache</param>
        public SurveillanceStationClient(IOptions<SurveillanceStationSettings> settings, IHttpClientFactory clientFactory, ILoggerFactory loggerFactory, IMemoryCacheWrapper cache) : base(clientFactory, loggerFactory, cache)
        {
            _settings = settings?.Value ?? new SurveillanceStationSettings();
        }

        /// <summary>
        /// Get the station settings
        /// </summary>
        protected SurveillanceStationSettings Settings => _settings;
        /// <summary>
        /// Get the api base URL
        /// </summary>
        protected override string ApiUri => _settings.BaseUrl;
        /// <summary>
        /// Syno token retreived during auth session
        /// </summary>
        public string SynoToken { get; internal set; }

        /// <inheritdoc/>
        protected override async Task PopulateOptionsToClientHeaders(HttpClient client, IRestApiRequestState state = null, CancellationToken cancellation = default)
        {
            await base.PopulateOptionsToClientHeaders(client, state, cancellation).ConfigureAwait(false);

            if (SynoToken.IsPresent())
            {
                Logger.LogDebug("Using preset SynoToken token.");
                Logger.LogTrace("Setting SynoToken token {synoToken} to access REST Api endpoint {apiUri}.", SynoToken, ApiUri);
                client.DefaultRequestHeaders.Add(StationConstants.ApiDefaults.TokenHeaderName, SynoToken);
            }
        }
        /// <inheritdoc/>
        protected async override Task<ApiClientException> ExceptionFromResponse(HttpResponseMessage response, CancellationToken canellation = default)
        {
            if (response != null)
                return await StationApiException.FromHttpResponseAsync(response, "REST Api error - " + (response?.StatusCode.ToString() ?? "0") + " (" + response.ReasonPhrase + ")");

            return null;
        }
    }
}
