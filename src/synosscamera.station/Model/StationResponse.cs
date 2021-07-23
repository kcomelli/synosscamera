namespace synosscamera.station.Model
{
    /// <summary>
    /// Station API response object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StationResponse<T>
    {
        /// <summary>
        /// Indicates success
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Depending on the called API this field returns the data
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// If success is false, you can find error inormation in this field
        /// </summary>
        public StationError Error { get; set; }
    }
}
