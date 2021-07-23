using System.ComponentModel.DataAnnotations;

namespace synosscamera.core.Configuration
{
    /// <summary>
    /// Api key data
    /// </summary>
    public class ApiKeyData
    {
        /// <summary>
        /// Api key
        /// </summary>
        [Required]
        public string ApiKey { get; set; }
        /// <summary>
        /// Name or identifier used in logs and claims
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Flag if this key is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}
