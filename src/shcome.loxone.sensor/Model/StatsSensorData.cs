using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace shcome.loxone.sensor.Model
{
    public class StatsSensorData
    {
        /// <summary>
        /// Extracted sensor name
        /// </summary>
        public string SensorName { get; set; }
        /// <summary>
        /// Extracted category
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Extracted room
        /// </summary>
        public string Room { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Parsed timespan string
        /// </summary>
        public string ReadingTimeSpan { get; set; }
        /// <summary>
        /// Only month and year is relevant
        /// </summary>
        public DateTime? ReadingTimeSpanAsDateTime => string.IsNullOrEmpty(ReadingTimeSpan) ? null : DateTime.ParseExact(ReadingTimeSpan, "yyyyMM", CultureInfo.InvariantCulture);
        /// <summary>
        /// Datas download uri
        /// </summary>
        public string DownloadUri { get; set; }
    }
}
