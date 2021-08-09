namespace synosscamera.core.Model.Dto.Camera
{
    /// <summary>
    /// Camera info response
    /// </summary>
    public class CameraInfoResponse : ApiResponse
    {
        /// <summary>
        /// Data of the camera
        /// </summary>
        public CameraDetailsSpecialized Data { get; set; }
    }
}
