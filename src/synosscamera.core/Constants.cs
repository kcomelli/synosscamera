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
        }
    }
}
