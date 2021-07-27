namespace synosscamera.core.Model.Dto.Camera
{
    /// <summary>
    /// Generic success response
    /// </summary>
    public class SuccessResponse : ApiResponse
    {
        /// <summary>
        /// Flag if the operation was successfully executed
        /// </summary>
        public bool Success { get; set; }
    }
}
