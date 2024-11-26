using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using shcome.core.Abstractions;
using System;
using shcome.core.Diagnostics;

namespace shcome.api.Infrastructure.Logging
{
    /// <summary>
    /// Default serilog log enricher
    /// </summary>
    public class SerilogLogEnricher : ILogEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="httpContextAccessor">HttpContext accessor</param>
        public SerilogLogEnricher(IHttpContextAccessor httpContextAccessor)
        {
            httpContextAccessor.CheckArgumentNull(nameof(httpContextAccessor));

            _httpContextAccessor = httpContextAccessor;
        }
        /// <inheritdoc/>
        public IDisposable PushProperty(string name, object value, bool destructureObjects = false, Microsoft.Extensions.Logging.ILogger logger = null)
        {
            if (_httpContextAccessor.HttpContext?.RequestServices != null)
            {
                try
                {
                    // enrich requestlog middleware
                    var diagnosticContext = _httpContextAccessor.HttpContext.RequestServices.GetService<IDiagnosticContext>();
                    diagnosticContext?.Set(name, value, destructureObjects);
                }
                catch (ObjectDisposedException ex)
                {
                    logger?.LogWarning(ex, "SerilogLogEnricher.PushProperty catched ObjectDisposedException");
                }
            }

            return LogContext.PushProperty(name, value, destructureObjects);
        }
    }
}
