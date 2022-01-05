using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.station.Model.ApiInfo
{
    /// <summary>
    /// Home mode info
    /// </summary>
    public class HomeModeInfo
    {
        /// <summary>
        /// Whether home mode is turned on or off
        /// </summary>
        public bool On { get; set; }
        /// <summary>
        /// Reason for the last switching of home mode 
        /// </summary>
        public int Reason { get; set; }
        /// <summary>
        /// Is the setting of recording enabled
        /// </summary>
        [JsonProperty("rec_schedule_on")]
        public bool RecordScheduleOn {get;set;}
        /// <summary>
        /// Is the setting of notification enabled.
        /// </summary>
        [JsonProperty("notify_on")]
        public bool NotifyOn { get; set; }
        /// <summary>
        /// Is the setting of streaming profile enabled.
        /// </summary>
        [JsonProperty("streaming_on")]
        public bool StreamingOn { get; set; }
        /// <summary>
        /// Is the setting of action rule enabled
        /// </summary>
        [JsonProperty("actrule_on")]
        public bool ActionRuleOn { get; set; }
        /// <summary>
        /// Is the home mode schedule switching enabled
        /// </summary>
        [JsonProperty("mode_schedule_on")]
        public bool ModeScheduleOn { get; set; }
        /// <summary>
        /// Recording schedule in home mode.
        /// </summary>
        [JsonProperty("rec_schedule")]
        public string RecordSchedule { get; set; }
    }
}
