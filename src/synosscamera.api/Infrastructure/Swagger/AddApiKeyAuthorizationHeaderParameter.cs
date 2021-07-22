using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using synosscamera.api.Infrastructure.Extensions;
using synosscamera.core;
using System;
using System.Collections.Generic;
using System.Linq;


namespace synosscamera.api.Infrastructure.Swagger
{
    /// <summary>
    /// Add authorization header parameter
    /// </summary>
    public class AddApiKeyAuthorizationHeaderParameter : IOperationFilter
    {
        /// <summary>
        /// Apply filter
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var filters = filterPipeline.Select(filterInfo => filterInfo.Filter)
                                        .Where(filter => filter is AuthorizeFilter);

            // Policy names map to scopes
            var requiredSchemes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.AuthenticationSchemes)
                .Distinct()
                .ToList();


            var allowAnonymous = context.ApiDescription.HasAttribute<AllowAnonymousAttribute>();

            if (!allowAnonymous && filters.Count() > 0)
            {
                if (filters.Any(filter => ((AuthorizeFilter)filter).Policy.AuthenticationSchemes.Contains(Constants.Security.ApiKeyAuthenticationScheme)))
                {
                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement()
                        {
                            {  new OpenApiSecurityScheme()
                                {
                                    Reference = new OpenApiReference() { Type = ReferenceType.SecurityScheme, Id = Constants.Security.ApiKeyAuthenticationScheme }
                                },
                                new string[]{ }
                            }
                        }
                    };
                }
            }
            else if (!allowAnonymous && requiredSchemes.Count() > 0)
            {
                if (requiredSchemes.Any(filter => filter == Constants.Security.ApiKeyAuthenticationScheme))
                {
                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement()
                        {
                            {  new OpenApiSecurityScheme()
                                {
                                    Reference = new OpenApiReference() { Type = ReferenceType.SecurityScheme, Id = Constants.Security.ApiKeyAuthenticationScheme }
                                },
                                new string[]{ }
                            }
                        }
                    };
                }
            }
        }
    }
}
