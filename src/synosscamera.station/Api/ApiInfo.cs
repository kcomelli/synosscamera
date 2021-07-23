using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using synosscamera.station.Configuration;
using synosscamera.station.Infrastructure;
using synosscamera.station.Model.ApiInfo;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Api
{
    /// <summary>
    /// Surveillance station SYNO.API.Info implementation
    /// </summary>
    public class ApiInfo : StationApiBase
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public ApiInfo(SurveillanceStationClient client, IOptions<SurveillanceStationSettings> settings, IDistributedCache distributedCache, ILoggerFactory loggerFactory)
            : base(client, settings, distributedCache, loggerFactory)
        {
        }

        public async Task<ApiInfoQueryResponse> GetApisAsync(string queryApiFilter = "SYNO.API.Auth,SYNO.SurveillanceStation.", CancellationToken cancellation = default)
        {
            var query = await GetUrl("Query", parameter: new System.Collections.Generic.Dictionary<string, object>()
            {
                { "query", queryApiFilter }
            }, cancellation: cancellation);

            var response = await Client.CallGetApiAsync<ApiInfoQueryResponse>(string.Empty, query, token: cancellation);

        }

        /// <inheritdoc/>
        protected override Task<string> ResolvePathForApi(CancellationToken cancellation = default)
        {
            return Task.FromResult("query.cgi");
        }
        /// <inheritdoc/>
        protected override Task<(int minVersion, int maxVersion)> ResolveVersionsForApi(CancellationToken cancellation = default)
        {
            return Task.FromResult((1, 1));
        }
    }
}
