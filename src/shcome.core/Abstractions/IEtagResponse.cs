using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shcome.core.Abstractions
{
    /// <summary>
    /// Internal interface for attaching an received Etag header value to a response
    /// </summary>
    public interface IEtagResponse
    {
        /// <summary>
        /// Received ETag
        /// </summary>
        string Etag { get; set; }
    }
}
