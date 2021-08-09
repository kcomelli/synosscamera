using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Model.Dto
{
    /// <summary>
    /// Input class for recording a hook request error
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Type of the error 
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
        [JsonConverter(converterType: typeof(StringEnumConverter))]
        public ApiErrorTypes ErrorType { get; set; }
        /// <summary>
        /// Number or code of the error
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ErrorCode { get; set; } = string.Empty;
        /// <summary>
        /// Error source (e.g. a property in cae of model binding errors)
        /// </summary>
        public string ErrorSource { get; set; }
        /// <summary>
        /// A human readable error message
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ErrorMessage { get; set; } = string.Empty;
        /// <summary>
        /// If this error was caused by an external system or component, the external error code may be stored in this property
        /// </summary>
        public string ExternalErrorCode { get; set; }
        /// <summary>
        /// If this error was caused by an external system or compoent, a specific error message may be stored in this property
        /// </summary>
        public string ExternalErrorMessage { get; set; }
    }
}
