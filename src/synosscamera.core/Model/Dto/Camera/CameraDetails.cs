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
    }
}
