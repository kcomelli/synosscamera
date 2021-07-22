using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core
{
    public static class Constants
    {
        /// <summary>
        /// Default headers for additional logic
        /// </summary>
        public static class DefaultHeaders
        {
            /// <summary>
            /// If this header is present, it will contain the milliseconds elapsed prioir of sending the response back to the client
            /// </summary>
            public const string ProcessTimeMilliseconds = "X-PROCESS-TIME-MILLISECONDS";
            /// <summary>
            /// Authorization header
            /// </summary>
            public const string Authorization = "Authorization";
        }

        /// <summary>
        /// Default headers for additional logic
        /// </summary>
        public static class Security
        {
            /// <summary>
            /// Apikey authentication scheme
            /// </summary>
            public const string ApiKeyAuthenticationScheme = "ApiKey";

            /// <summary>
            /// Api key token name
            /// </summary>
            public const string ApiTokenName = "synoss_apikey";
            /// <summary>
            /// Api user name
            /// </summary>
            public const string ApiUserName = "apiuser";

            /// <summary>
            /// Constant claims
            /// </summary>
            public static class Claims
            {
                /// <summary>
                /// Claims issuer
                /// </summary>
                public const string BackendClaimsIssuer = "http://topmindSecurity/smBackend";

                /// <summary>
                /// Claim type of subject or id of a claims principal
                /// </summary>
                public const string Subject = "sub";
                
            }
        }
    }
}
