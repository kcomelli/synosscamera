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
    }
}
