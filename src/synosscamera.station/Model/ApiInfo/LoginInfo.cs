using Newtonsoft.Json;

namespace synosscamera.station.Model.ApiInfo
{
    /// <summary>
    /// Login info from API
    /// </summary>
    public class LoginInfo
    {
        /// <summary>
        /// Session ID, pass this value by HTTP argument "_sid" or Cookie argument.
        /// </summary>
        public string Sid { get; set; }
        /// <summary>
        /// Device id, use to skip OTP checking.
        /// </summary>
        public string Did { get; set; }
        /// <summary>
        /// If CSRF enabled in DSM, pass this value by HTTP argument "SynoToken"
        /// </summary>
        public string Synotoken { get; set; }
        /// <summary>
        /// Login through app portal
        /// </summary>
        [JsonProperty("is_portal_port")]
        public string IsPortalPort { get; set; }
    }
}
