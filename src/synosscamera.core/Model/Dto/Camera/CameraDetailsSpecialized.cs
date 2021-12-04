using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace synosscamera.core.Model.Dto.Camera
{
    /// <summary>
    /// Camera special info
    /// </summary>
    public class CameraDetailsSpecialized
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
        [JsonConverter(converterType: typeof(StringEnumConverter))]
        public RecordingState RecordingStatus { get; set; }
        /// <summary>
        /// Url of taking a snapshot
        /// </summary>
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
        /// <summary>
        /// Detail infos
        /// </summary>
        public CameraDetailInfo DetailInfo { get; set; }
    }
}
