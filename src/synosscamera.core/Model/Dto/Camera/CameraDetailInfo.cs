using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Model.Dto.Camera
{
    /// <summary>
    /// Camera detail info object
    /// </summary>
    public class CameraDetailInfo
    {
        /// <summary>
        /// Cam recording schedule
        /// </summary>
        public int[,] CamSchedule { get; set; }
    }
}
