using System.Collections.Generic;

namespace synosscamera.core.Model.Dto.Camera
{
    /// <summary>
    /// Cameralist response
    /// </summary>
    public class CameraListResponse : ApiResponse
    {
        /// <summary>
        /// Camere details
        /// </summary>
        public List<CameraDetails> Cameras { get; set; }
    }
}
