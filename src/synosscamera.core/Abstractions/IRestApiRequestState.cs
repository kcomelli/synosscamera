using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Abstractions
{
    /// <summary>
    /// Interface you can implement to create a per request state object for the rest api client
    /// </summary>
    public interface IRestApiRequestState
    {
        /// <summary>
        /// ETags which will be sent in the If-None-Match header
        /// </summary>
        List<string> NonMatchEtags { get; set; }
        /// <summary>
        /// ETags which will be sent in the If-Match header
        /// </summary>
        List<string> MatchEtags { get; set; }

        /// <summary>
        /// Request properties generated during execution. 
        /// Your custom message handlers can use this dictionary to share and pass state
        /// </summary>
        Dictionary<string, object> RequestProperties { get; set; }

        /// <summary>
        /// Return status code
        /// </summary>
        HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Response header collection
        /// </summary>
        HttpResponseHeaders ResponseHeaders { get; set; }
    }
}
