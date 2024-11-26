using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shcome.core.Abstractions
{
    /// <summary>
    /// Log event enricher
    /// </summary>
    public interface ILogEnricher
    {
        /// <summary>
        /// Push a property onto the context, returning an System.IDisposable that must later
        /// be used to remove the property, along with any others that may have been pushed
        /// on top of it and not yet popped. The property must be popped from the same thread/logical
        /// call context.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="destructureObjects">If true, and the value is a non-primitive, non-array type, then the value will
        /// be converted to a structure; otherwise, unknown types will be converted to scalars, which are generally stored as strings.</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        IDisposable PushProperty(string name, object value, bool destructureObjects = false, ILogger logger = null);
    }
}
