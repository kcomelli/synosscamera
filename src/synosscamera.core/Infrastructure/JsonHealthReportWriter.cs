using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace synosscamera.core.Infrastructure
{
    /// <summary>
    /// Json health report writer used for asp.net core health endpoint
    /// </summary>
    public static class JsonHealthReportWriter
    {
        /// <summary>
        /// Convert a <see cref="HealthReport"/> into an json object
        /// </summary>
        /// <param name="result"><see cref="HealthReport"/> to convert.</param>
        /// <returns>A new <see cref="JObject"/> instance representing the healthreport as json data</returns>
        public static JObject ReportToJsonObject(HealthReport result)
        {
            return new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));
        }
        /// <summary>
        /// Write a <see cref="HealthReport"/> to the response stream
        /// </summary>
        /// <param name="context">Current <see cref="HttpContext"/> to write to.</param>
        /// <param name="result"><see cref="HealthReport"/> to write to the response.</param>
        /// <returns>An awaitable task</returns>
        public static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";

            var json = ReportToJsonObject(result);

            return context.Response.WriteAsync(
                json.ToString(Newtonsoft.Json.Formatting.Indented));
        }
    }
}
