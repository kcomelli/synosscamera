using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace shcome.loxone.sensor.EntityObjects
{
    [Table("sensor_value", Schema = "public")]
    public partial class SensorValue
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("sensorId")]
        public int SensorId { get; set; }
        [Required]
        [Column("descriminator", TypeName = "character varying")]
        public string Descriminator { get; set; }
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        [Column("reading")]
        public double? Reading { get; set; }
        [Column("reading_normalized")]
        public double? ReadingNormalized { get; set; }

        [ForeignKey(nameof(SensorId))]
        [InverseProperty("SensorValues")]
        public virtual Sensor Sensor { get; set; }
    }
}
