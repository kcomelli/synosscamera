using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace synosscamera.api.Infrastructure.Authentication
{
    /// <summary>
    /// Post configuration callback handler will ensure option settings
    /// </summary>
    public class ApiKeyAuthenticationOptionsPostConfigureOptions : IPostConfigureOptions<ApiKeyAuthenticationOptions>
    {
        /// <summary>
        /// Callback after configuration done
        /// </summary>
        /// <param name="name">Name of the authentication</param>
        /// <param name="options">Options</param>
        public void PostConfigure(string name, ApiKeyAuthenticationOptions options)
        {
            if (string.IsNullOrEmpty(options.Realm))
            {
                throw new InvalidOperationException("Realm must be provided in options");
            }
            if (options.AllowCustomHeader && string.IsNullOrEmpty(options.CustomHeaderName))
            {
                throw new InvalidOperationException("Custom header name must be provided in options");
            }
        }
    }
}
