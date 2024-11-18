using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace shcome.loxone.sensor.Model
{
    [XmlRoot("Statistics")]
    public class StatisticReadings
    {
        
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public int NumOutputs { get; set; }
        [XmlAttribute]
        public string Outputs { get; set; }

        [XmlElement("S")]
        public StatisticRow[] Rows { get; set; }
    }
}
