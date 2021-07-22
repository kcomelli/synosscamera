using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace synosscamera.api.Infrastructure.Mvc
{
    /// <summary>
    /// Separated query string value provider factory
    /// </summary>
    public class SeparatedQueryStringValueProviderFactory : IValueProviderFactory
    {
        private readonly string _separator;
        private readonly string _key;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="separator">Separator</param>
        public SeparatedQueryStringValueProviderFactory(string separator) : this(null, separator)
        {
        }
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="key">Provider key</param>
        /// <param name="separator">Separator</param>
        public SeparatedQueryStringValueProviderFactory(string key, string separator)
        {
            _key = key;
            _separator = separator;
        }
        /// <summary>
        /// Create a new value provider
        /// </summary>
        /// <param name="context">Value provider context</param>
        /// <returns></returns>
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            context.ValueProviders.Insert(0, new SeparatedQueryStringValueProvider(_key, context.ActionContext.HttpContext.Request.Query, _separator));
            return Task.CompletedTask;
        }
    }
}
