namespace synosscamera.station.Configuration
{
    /// <summary>
    /// surveillance station settings
    /// </summary>
    public class SurveillanceStationSettings
    {
        /// <summary>
        /// Base url of api
        /// </summary>
        public string BaseUrl { get; set; }
        /// <summary>
        /// Optional session name used to connect to sation API
        /// </summary>
        public string SessionNameForStation { get; set; }
        /// <summary>
        /// Username to connect to station
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password to connect to station
        /// </summary>
        public string Password { get; set; }
    }
}
