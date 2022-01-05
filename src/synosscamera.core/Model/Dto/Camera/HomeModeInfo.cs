using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Model.Dto.Camera
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
        public bool RecordScheduleOn { get; set; }
        /// <summary>
        /// Is the setting of notification enabled.
        /// </summary>
        public bool NotifyOn { get; set; }
        /// <summary>
        /// Is the setting of streaming profile enabled.
        /// </summary>
        public bool StreamingOn { get; set; }
        /// <summary>
        /// Is the setting of action rule enabled
        /// </summary>
        public bool ActionRuleOn { get; set; }
        /// <summary>
        /// Is the home mode schedule switching enabled
        /// </summary>
        public bool ModeScheduleOn { get; set; }
        /// <summary>
        /// Recording schedule in home mode.
        /// </summary>
        public string RecordSchedule { get; set; }
    }
}
