using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Model.Dto
{
    /// <summary>
    /// Default api error response object
    /// </summary>
    public class ApiErrorResponse : ApiResponse
    {
        /// <summary>
        /// A property allowing clients to check if the JSON response is an error from the api without
        /// iterating through the errors collection
        /// </summary>
        [Required]
        public bool IsApiError { get; set; } = true;

        /// <summary>
        /// Api error list
        /// </summary>
        [Required]
        public ApiError[] Errors { get; set; }
    }
}
