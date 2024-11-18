using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shcome.loxone.sensor.Config
{
    public class SensorOptions
    {
        public List<SensorEndpoint> Endpoints { get; set; } = new List<SensorEndpoint>();
        public int DefaultImportFrequencyInMs { get; set; } = 1000 * 60 * 10; // 10min intervall
    }
}
