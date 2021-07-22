using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace synosscamera.api.Infrastructure.Swagger
{
    /// <summary>
    /// Query parameter filter for arrays
    /// </summary>
    public class QueryArrayParamFilter : IParameterFilter
    {
        /// <summary>
        /// Apply filter
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <param name="context">Filter context</param>
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (parameter.In.HasValue && parameter.In.Value == ParameterLocation.Query)
            {
                if (parameter.Schema?.Type == "array")
                {
                    // set CSV format for array params (see: http://swagger.io/specification/) to support comma separated parameters
                    parameter.Schema.Extensions.Add("collectionFormat", new OpenApiString("csv"));
                }
            }
        }
    }
}
