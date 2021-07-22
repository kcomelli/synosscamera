using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Extensions
{
    /// <summary>
    /// Disctionary extensions
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Split a list by the number of items specified in <paramref name="nSize"/> 
        /// </summary>
        /// <typeparam name="TKey">Data type of key</typeparam>
        /// <typeparam name="TData">Data type of data</typeparam>
        /// <param name="source">Source list</param>
        /// <param name="nSize">Max size per splitted list</param>
        /// <returns>Enumerable of lists with the a maximum number of <paramref name="nSize"/> elements.</returns>
        public static IEnumerable<IDictionary<TKey, TData>> Split<TKey, TData>(this IDictionary<TKey, TData> source, int nSize)
        {
            int counter = 0;

            return source
                    .GroupBy(x => counter++ / nSize)
                    .Select(g => g.ToDictionary(h => h.Key, h => h.Value));
        }
    }
}
