using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace synosscamera.api.Infrastructure.Swagger
{
    /// <summary>
    /// Internal operation filter extensions
    /// </summary>
    internal static class OperationFilterContextExtensions
    {
        /// <summary>
        /// Get controller and operation attributes from context
        /// </summary>
        /// <typeparam name="T">Attribute to retrieve.</typeparam>
        /// <param name="context">Operation filter context to use.</param>
        /// <returns>An enumerable of attribute instances found on controller or operation level.</returns>
        public static IEnumerable<T> GetControllerAndActionAttributes<T>(this OperationFilterContext context) where T : Attribute
        {
            var controllerAttributes = context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes<T>();
            var actionAttributes = context.MethodInfo.GetCustomAttributes<T>();

            var result = new List<T>(controllerAttributes);
            result.AddRange(actionAttributes);
            return result;
        }
    }
}
