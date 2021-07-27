using Newtonsoft.Json;

namespace synosscamera.station.Model.ApiInfo
{
    /// <summary>
    /// Camera info object
    /// </summary>
    public class CameraInfo
    {
        /// <summary>
        /// Id of the camera
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Status of the camera
        /// </summary>
        public CameraStatus Status { get; set; }
        /// <summary>
        /// Digital output number
        /// </summary>
        [JsonProperty("DONum")]
        public int DONum { get; set; }
        /// <summary>
        /// Digital input number
        /// </summary>
        [JsonProperty("DINum")]
        public int DINum { get; set; }
        /// <summary>
        /// Name of the camera
        /// </summary>
        [JsonProperty("newName")]
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
        /// Model of the camera
        /// </summary>
        public string Model { get; set; }
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
        /// Replacement strategy parameter
        /// </summary>
        public int RecordingKeepDays { get; set; }
        /// <summary>
        /// Replacement strategy parameter
        /// </summary>
        public int RecordingKeepSize { get; set; }
        /// <summary>
        /// TV stanbndard
        /// </summary>
        public TvStandard TvStandard { get; set; }
        /// <summary>
        /// Video codec of camera
        /// </summary>
        public VideoCodec VideoCodec { get; set; }
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
