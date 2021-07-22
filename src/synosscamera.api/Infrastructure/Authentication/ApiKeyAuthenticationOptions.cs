using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace synosscamera.api.Infrastructure.Authentication
{
    /// <summary>
    /// StoreMind API key authentication options
    /// </summary>
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Realm is needed for the cryptography protection space
        /// </summary>
        public string Realm { get; set; }
        /// <summary>
        /// Set this to true if the authorization key may come from a custom header.
        /// If this is set to true <see cref="CustomHeaderName"/> must be set.
        /// </summary>
        public bool AllowCustomHeader { get; set; } = false;
        /// <summary>
        /// Custom header name to get the key from if Authroization header is missing
        /// </summary>
        public string CustomHeaderName { get; set; }
        /// <summary>
        /// Set this to true if the token or api key can be sent via query parameter
        /// </summary>
        public bool AllowTokenInQueryString { get; set; } = true;
        /// <summary>
        /// Default queryparameter name if scanning the for the token
        /// </summary>
        public string TokenQueryParameterName { get; set; } = "apikey";
        /// <summary>
        /// Set this to true if the key may be read from the request path.
        /// If this is the case, the key must be the last path segment.
        /// </summary>
        public bool ReadKeyFromPath { get; set; } = false;
    }
}
