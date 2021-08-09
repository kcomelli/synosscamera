using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace synosscamera.core.Model.Dto.Camera
{
    /// <summary>
    /// Camera details
    /// </summary>
    public class CameraDetails
    {
        /// <summary>
        /// Id of the camera
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Status of the camera
        /// </summary>
        [JsonConverter(converterType: typeof(StringEnumConverter))]
        public CameraState Status { get; set; }
        /// <summary>
        /// Name of the camera
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// IP Address of the camera
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// Port of the camera
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Vendor of the camera
        /// </summary>
        public string Vendor { get; set; }
        /// <summary>
        /// Camera channel id
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// Low profile enabled - yes or no
        /// </summary>
        public bool EnableLowProfile { get; set; }
        /// <summary>
        /// The used stream number of medium bandwidth profile
        /// </summary>
        public bool EnableMediumProfile { get; set; }
        /// <summary>
        /// The used stream number of high bandwidth profile
        /// </summary>
        public bool EnableHighProfile { get; set; }
        /// <summary>
        /// The used stream number of high bandwidth profile
        /// </summary>
        public int HighProfileStreamNo { get; set; }
        /// <summary>
        /// The used stream number of medium bandwidth profile
        /// </summary>
        public int MediumProfileStreamNo { get; set; }
        /// <summary>
        /// The used stream number of low bandwidth profile
        /// </summary>
        public int LowProfileStreamNo { get; set; }
        /// <summary>
        /// Record time in minutes
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// Pre-Record time in minutes
        /// </summary>
        public int PreRecordTime { get; set; }
        /// <summary>
        /// Post-Record time in minutes
        /// </summary>
        public int PostRecordTime { get; set; }
    }
}
