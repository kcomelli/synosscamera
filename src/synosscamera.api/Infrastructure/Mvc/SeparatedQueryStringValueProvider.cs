using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace synosscamera.api.Infrastructure.Mvc
{
    /// <summary>
    /// Separated value provider for querystring parameter
    /// </summary>
    public class SeparatedQueryStringValueProvider : QueryStringValueProvider
    {
        private readonly string _key;
        private readonly string _separator;
        private readonly IQueryCollection _values;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="values">Values collection</param>
        /// <param name="separator">Separator</param>
        public SeparatedQueryStringValueProvider(IQueryCollection values, string separator)
            : this(null, values, separator)
        {
        }
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="key">Provider key</param>
        /// <param name="values">Values collection</param>
        /// <param name="separator">Separator</param>
        public SeparatedQueryStringValueProvider(string key, IQueryCollection values, string separator)
            : base(BindingSource.Query, values, CultureInfo.InvariantCulture)
        {
            _key = key;
            _values = values;
            _separator = separator;
        }
        /// <summary>
        /// Get a value
        /// </summary>
        /// <param name="key">>Value object key</param>
        /// <returns></returns>
        public override ValueProviderResult GetValue(string key)
        {
            var result = base.GetValue(key);

            if (_key != null && _key != key)
            {
                return result;
            }

            if (result != ValueProviderResult.None && result.Values.Any(x => x.IndexOf(_separator, StringComparison.OrdinalIgnoreCase) > 0))
            {
                var splitValues = new StringValues(result.Values
                    .SelectMany(x => x.Split(new[] { _separator }, StringSplitOptions.None)).ToArray());
                return new ValueProviderResult(splitValues, result.Culture);
            }

            return result;
        }
    }
}
