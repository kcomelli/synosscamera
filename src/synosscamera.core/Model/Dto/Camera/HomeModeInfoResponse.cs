using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Model.Dto.Camera
{
    /// <summary>
    /// Homemode info response
    /// </summary>
    public class HomeModeInfoResponse : ApiResponse
    {
        /// <summary>
        /// Data of the homemode settings
        /// </summary>
        public HomeModeInfo Data { get; set; }
    }
}
