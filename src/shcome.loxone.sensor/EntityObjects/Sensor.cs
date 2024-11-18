using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace shcome.loxone.sensor.EntityObjects
{
    [Table("sensor", Schema = "public")]
    public partial class Sensor
    {
        public Sensor()
        {
            SensorValues = new HashSet<SensorValue>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("description", TypeName = "character varying")]
        public string Description { get; set; }
        [Column("type", TypeName = "character varying")]
        public string Type { get; set; }
        [Column("category", TypeName = "character varying")]
        public string Category { get; set; }
        [Column("room", TypeName = "character varying")]
        public string Room { get; set; }

        [InverseProperty("Sensor")]
        public virtual ICollection<SensorValue> SensorValues { get; set; }
    }
}
