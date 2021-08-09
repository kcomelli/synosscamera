using synosscamera.core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Infrastructure.Http
{
    /// <summary>
    /// Default rest api request state
    /// </summary>
    public class DefaultRequestApiState : IRestApiRequestState
    {
        /// <summary>
        /// Non mathing etags for filtering
        /// </summary>
        public List<string> NonMatchEtags { get; set; } = new List<string>();
        /// <summary>
        /// Match etags for filtering
        /// </summary>
        public List<string> MatchEtags { get; set; } = new List<string>();
        /// <summary>
        /// Status code returned
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Headers returned
        /// </summary>
        public HttpResponseHeaders ResponseHeaders { get; set; }
        /// <summary>
        /// Request properties used
        /// </summary>
        public Dictionary<string, object> RequestProperties { get; set; }
    }
}
