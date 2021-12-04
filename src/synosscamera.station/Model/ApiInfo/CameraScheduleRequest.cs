using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.station.Model.ApiInfo
{
    /// <summary>
    /// Camera schedule request
    /// </summary>
    public class CameraScheduleRequest
    {
        /// <summary>
        /// Record schedule information to set
        /// </summary>
        [JsonProperty("recordSchedule")]
        public int[,] RecordSchedule { get; set; }
    }
}
