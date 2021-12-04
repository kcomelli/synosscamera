using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.station.Api
{
    /// <summary>
    /// Record schedule
    /// </summary>
    public enum RecordSchedule
    {
        /// <summary>
        /// Record schedule off
        /// </summary>
        Off = 0,
        /// <summary>
        /// Continuous recording
        /// </summary>
        Continuous = 1,
        /// <summary>
        /// Motion detection recording
        /// </summary>
        MotionDetection = 2
    }
}
