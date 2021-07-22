using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using synosscamera.core.Infrastructure;
using synosscamera.core.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.DependencyInjection
{
    /// <summary>
    /// Application builder extensions
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add a process time middleware which measures the request execution time and returns it via a response header
        /// </summary>
        /// <param name="app">Application builder instance</param>
        /// <returns>Application builder instance</returns>
        public static IApplicationBuilder UseProcessTimeReporting(this IApplicationBuilder app)
        {
            app.UseMiddleware<ProcessTimeMiddleware>();

            return app;
        }

        /// <summary>
        /// Add a simple ping-pong endpoint used to verify if the server did start up
        /// </summary>
        /// <param name="app">Application builder instance</param>
        /// <param name="endpoint">endpoint to use (defaults to "/ping")</param>
        /// <returns>Application builder instance</returns>
        public static IApplicationBuilder UsePingEndpoint(this IApplicationBuilder app, string endpoint = "/ping")
        {
            app.MapWhen(context => context.Request.Path.StartsWithSegments(endpoint, StringComparison.OrdinalIgnoreCase), appBuilder =>
            {
                appBuilder.Run(async conext => await conext.Response.WriteAsync("pong", conext.RequestAborted));
            });
            return app;
        }

        /// <summary>
        /// Add a simple ping-pong endpoint used to verify if the server did start up
        /// </summary>
        /// <param name="app">Application builder instance</param>
        /// <param name="endpoint">endpoint to use (defaults to "/health")</param>
        /// <returns>Application builder instance</returns>
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app, string endpoint = "/health")
        {
            app.MapWhen(context => context.Request.Path.StartsWithSegments(endpoint, StringComparison.OrdinalIgnoreCase), appBuilder =>
            {
                appBuilder.UseRouting();
                appBuilder.UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks(endpoint, new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
                    {
                        ResponseWriter = JsonHealthReportWriter.WriteResponse
                    });
                });
            });
            return app;
        }

    }
}
