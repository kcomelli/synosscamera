using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.station.Configuration
{
    /// <summary>
    /// surveillance station settings
    /// </summary>
    public class SurveillanceStationSettings
    {
        /// <summary>
        /// Base url of api
        /// </summary>
        public string BaseUrl { get; set; }
        /// <summary>
        /// Username to connect to station
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password to connect to station
        /// </summary>
        public string Password { get; set; }
    }
}
