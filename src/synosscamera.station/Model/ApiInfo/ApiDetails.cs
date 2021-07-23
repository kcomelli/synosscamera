namespace synosscamera.station.Model.ApiInfo
{
    /// <summary>
    /// Api info details
    /// </summary>
    public class ApiDetails
    {
        /// <summary>
        /// Max version that can be used
        /// </summary>
        public int MaxVersion { get; set; }
        /// <summary>
        /// Min version that must be used
        /// </summary>
        public int MinVersion { get; set; }
        /// <summary>
        /// Api URL path to add
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Request format if soecified
        /// </summary>
        public string RequestFormat { get; set; } = null;
    }
}
