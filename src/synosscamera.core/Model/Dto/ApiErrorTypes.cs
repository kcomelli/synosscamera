using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Model.Dto
{
    /// <summary>
    /// Enumeration defining possible hook request errors
    /// </summary>
    public enum ApiErrorTypes
    {
        /// <summary>
        /// Unknown error source
        /// </summary>
        Unknown,
        /// <summary>
        /// Error happend based on content
        /// </summary>
        ContentError,
        /// <summary>
        /// Unexpected internal server error
        /// </summary>
        InternalServerError,
        /// <summary>
        /// Data error or fonclict error
        /// </summary>
        DataError,
        /// <summary>
        /// Error from model binding validation
        /// </summary>
        ModelValidation,
        /// <summary>
        /// Error from header validation 
        /// </summary>
        HeaderValidation,
        /// <summary>
        /// Error happened by an external API call
        /// </summary>
        ExternalRestApi
    }
}
