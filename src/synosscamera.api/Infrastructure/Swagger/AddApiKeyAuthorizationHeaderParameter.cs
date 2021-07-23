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
            if(context.GetControllerAndActionAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var actionAttributes = context.GetControllerAndActionAttributes<AuthorizeAttribute>();

            if (!actionAttributes.Any())
            {
                return;
            }

            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized. Authentication is required in order to use this operation." });
            }

            if (!operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden. The current authenticated client or user does not have permissions to use this operation." });
            }


            var requiredSchemes = actionAttributes
                .Select(attr => attr.AuthenticationSchemes)
                .Distinct()
                .ToList();


            if (requiredSchemes?.Any(filter => filter == Constants.Security.ApiKeyAuthenticationScheme) == true)
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
