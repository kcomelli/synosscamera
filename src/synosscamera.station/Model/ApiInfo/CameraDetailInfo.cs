using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.station.Model.ApiInfo
{
    /// <summary>
    /// Camera detail info object
    /// </summary>
    public class CameraDetailInfo
    {
        /// <summary>
        /// Cam recording schedule
        /// </summary>
        [JsonProperty("camSchedule")]
        public int[,] CamSchedule { get; set; }
    }
}
