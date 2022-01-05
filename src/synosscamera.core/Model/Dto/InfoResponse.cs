using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Model.Dto
{
    /// <summary>
    /// Info 
    /// </summary>
    public class InfoResponse : ApiResponse
    {
        /// <summary>
        /// Api version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Root path
        /// </summary>
        public string WebRootPath { get; set; }
        /// <summary>
        /// Content path
        /// </summary>
        public string ContentPath { get; set; }
        /// <summary>
        /// The station endpoint used
        /// </summary>
        public string StationEndpoint { get; set; }
    }
}
