using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace shcome.loxone.sensor.Model
{
    public class StatisticRow
    {
        private System.DateTime? _timeStamp;

        public DateTime? Timestamp => _timeStamp;

        [XmlAttribute("T")]
        public string TimestampRaw
        {
            get
            {
                return _timeStamp?.ToString("MM-dd-yyyy HH:mm:ss");
            }
            set
            {
                _timeStamp = System.DateTime.Parse(value);
            }
        }

        [XmlAttribute("V")]
        public double Value { get; set; }
        [XmlAttribute("V2")]
        public double Value2 { get; set; }
        [XmlAttribute("V3")]
        public double Value3 { get; set; }
        [XmlAttribute("V4")]
        public double Value4 { get; set; }
        [XmlAttribute("V5")]
        public double Value5 { get; set; }
    }
}
