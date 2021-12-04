using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using synosscamera.core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Infrastructure.Http
{
    /// <summary>
    /// Json content client 
    /// </summary>
    public abstract class JsonRestApiClientBase : RestApiClientBase
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="loggerFactory">Logger factory for creating loggers</param>
        /// <param name="cache">Local memory cache</param>
        protected JsonRestApiClientBase(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory, IMemoryCacheWrapper cache) : base(clientFactory, loggerFactory, cache)
        {

        }

        /// <inheritdoc/>
        protected override HttpContent GetContent(object data)
        {
            if (data == null)
                return new StringContent(string.Empty, Encoding.UTF8, "application/json");

            return new StringContent(ToJson(data), Encoding.UTF8, "application/json");
        }
    }
}
