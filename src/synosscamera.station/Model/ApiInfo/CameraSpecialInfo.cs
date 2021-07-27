using Newtonsoft.Json;

namespace synosscamera.station.Model.ApiInfo
{
    /// <summary>
    /// Queried camera defail information
    /// </summary>
    public class CameraSpecialInfo
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
        /// Name of the camera
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// IP Address of the camera
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Port of the camera
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Is camera deleted
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// Is camera muted
        /// </summary>
        public bool Muted { get; set; }
        /// <summary>
        /// Recording status of the camera
        /// </summary>
        [JsonProperty("recStatus")]
        public RecordingStatus RecordingStatus { get; set; }
        /// <summary>
        /// Url of taking a snapshot
        /// </summary>
        [JsonProperty("snapshot_path")]
        public string SnapshotUrl { get; set; }
        /// <summary>
        /// Camera volume
        /// </summary>
        public int Volume { get; set; }
        /// <summary>
        /// Is video flipped
        /// </summary>
        public bool VideoFlip { get; set; }
        /// <summary>
        /// Is video mirrored
        /// </summary>
        public bool VideoMirror { get; set; }
    }
}
