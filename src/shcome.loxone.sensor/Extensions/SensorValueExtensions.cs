using shcome.loxone.sensor.EntityObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shcome.loxone.sensor.Extensions
{
    public static class SensorValueExtensions
    {
        public static bool Equals(this SensorValue v, SensorValue compare)
        {
            if (v == null && compare == null)
                return true;

            if(v!= null && compare != null)
            {
                return v.Descriminator == compare.Descriminator &&
                       v.Reading == compare.Reading &&
                       v.ReadingNormalized == compare.ReadingNormalized &&
                       v.Timestamp == compare.Timestamp &&
                       (v.Sensor == compare.Sensor || v.SensorId == compare.SensorId);
            }

            return false;
        }
    }
}
