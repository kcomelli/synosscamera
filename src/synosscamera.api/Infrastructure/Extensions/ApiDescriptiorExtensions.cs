using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;

namespace synosscamera.api.Infrastructure.Extensions
{
    /// <summary>
    /// Extensions for API Descriptors
    /// </summary>
    public static class ApiDescriptiorExtensions
    {
        /// <summary>
        /// Check if an api descriptor has defined an attribute on controller or method level
        /// </summary>
        /// <typeparam name="TAttribute">Attribute type to check</typeparam>
        /// <param name="descriptor">Input descriptor to check</param>
        /// <returns>True if the attribute is present.</returns>
        public static bool HasAttribute<TAttribute>(this ApiDescription descriptor)
        {
            if (descriptor != null)
            {
                var ctrlDescriptor = descriptor.ActionDescriptor as ControllerActionDescriptor;

                if (ctrlDescriptor != null)
                    return ctrlDescriptor.MethodInfo.GetCustomAttributes(typeof(TAttribute), true)?.Count() > 0;
            }

            return false;
        }
    }
}
