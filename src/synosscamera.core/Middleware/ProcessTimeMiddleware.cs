using Microsoft.AspNetCore.Http;
using synosscamera.core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Middleware
{
    /// <summary>
    /// Middleware sending request processing time as a header to the client
    /// </summary>
    public class ProcessTimeMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="next">Next middleware in the pipeline</param>
        public ProcessTimeMiddleware(RequestDelegate next)
        {
            next.CheckArgumentNull(nameof(next));

            _next = next;
        }

        /// <summary>
        /// Middleware gets invoked
        /// </summary>
        /// <param name="context">On this HttpContext</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var dtStart = DateTime.UtcNow;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(Constants.DefaultHeaders.ProcessTimeMilliseconds, new[] { (DateTime.UtcNow - dtStart).TotalMilliseconds.ToString() });
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
