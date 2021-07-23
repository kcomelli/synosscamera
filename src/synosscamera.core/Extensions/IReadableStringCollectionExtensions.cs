using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Extensions
{
    /// <summary>
    /// Readable string collection extensions
    /// </summary>
    public static class IReadableStringCollectionExtensions
    {
        /// <summary>
        /// Converts an enumerable key value pair to a <see cref="NameValueCollection"/> instance
        /// </summary>
        /// <param name="collection">Collection to convert</param>
        /// <returns>A <see cref="NameValueCollection"/> representatio of the collection</returns>
        public static NameValueCollection AsNameValueCollection(this IEnumerable<KeyValuePair<string, StringValues>> collection)
        {
            var nv = new NameValueCollection();

            foreach (var field in collection)
            {
                nv.Add(field.Key, field.Value.First());
            }

            return nv;
        }

        /// <summary>
        /// Converts an dictionary to a <see cref="NameValueCollection"/> instance
        /// </summary>
        /// <param name="collection">Dictionary to convert</param>
        /// <returns>A <see cref="NameValueCollection"/> representatio of the collection</returns>
        public static NameValueCollection AsNameValueCollection(this IDictionary<string, StringValues> collection)
        {
            var nv = new NameValueCollection();

            foreach (var field in collection)
            {
                nv.Add(field.Key, field.Value.First());
            }

            return nv;
        }
    }
}
