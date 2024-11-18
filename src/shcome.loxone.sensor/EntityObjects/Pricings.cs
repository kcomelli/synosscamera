using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shcome.loxone.sensor.EntityObjects
{
    public class Pricings
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("descriminator", TypeName = "character varying")]
        public string Descriminator { get; set; }
        [Column("valid_from")]
        public DateTime? ValidFrom { get; set; }
        [Column("price")]
        public double Price { get; set; }
        [Column("unit", TypeName = "character varying")]
        public string Unit { get; set; } // eg kW/h
        [Column("progression_threshold")]
        public double ProgressionThreshold { get; set; }

    }
}
