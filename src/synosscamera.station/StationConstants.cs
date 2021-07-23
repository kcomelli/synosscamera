using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.station
{
    /// <summary>
    /// Station constants
    /// </summary>
    public static class StationConstants
    {
        /// <summary>
        /// Error codes
        /// </summary>
        public static class ErrorCodes
        {
            /// <summary>
            /// Unknown error occured
            /// </summary>
            public const int UnknownError = 100;
            /// <summary>
            /// Invalid parameter and/or value supplied
            /// </summary>
            public const int InvalidParameters = 101;
            /// <summary>
            /// The requested api does not exist
            /// </summary>
            public const int ApiDoesNotExist = 102;
            /// <summary>
            /// The called method does not exist
            /// </summary>
            public const int MethodDoesNotExist = 103;
            /// <summary>
            /// This version of the api is not supported
            /// </summary>
            public const int ApiVersionNotSupported = 104;
            /// <summary>
            /// User has insufficient priviliges
            /// </summary>
            public const int InsufficientUserPriviliges = 105;
            /// <summary>
            /// Connection timed out
            /// </summary>
            public const int ConnectionTimeout = 106;
            /// <summary>
            /// Multiple logins of this user detected
            /// </summary>
            public const int MultipleLoginDetected = 107;
        }
    }
}
