using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using synosscamera.api.Infrastructure.Authentication;
using synosscamera.api.Infrastructure.Mvc;
using synosscamera.api.Infrastructure.Swagger;
using synosscamera.core;
using synosscamera.core.DependencyInjection;
using synosscamera.core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;

namespace synosscamera.api
{
    public class Startup
    {
        /// <summary>
        /// Configuration access
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// Hosting environment access
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Environment = env;

            if (env.WebRootPath.IsMissing())
            {
                // fix for linux environments
                env.WebRootPath = System.IO.Path.Combine(env.ContentRootPath, "wwwroot");
            }

            Configuration = configuration;
            ConfigureStructuredLogging(env);

            // may become handy on high load scenarios where we can fine-tune threadding behaviour
            ThreadUtil.ConfigureThreadPool(Configuration, null);
        }

        private void ConfigureStructuredLogging(IWebHostEnvironment env)
        {
            var orchestrator = System.Environment.GetEnvironmentVariable("ASPNETCORE_ORCHESTRATOR");
            var orchestratorService = System.Environment.GetEnvironmentVariable("ASPNETCORE_ORCHESTRATOR_SERVICE") ?? "synosscamera-api";

            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("api-host", System.Environment.MachineName)
            .Enrich.WithProperty("api-environment", env.EnvironmentName)
            .Enrich.WithProperty("api-version", Assembly.GetEntryAssembly().GetName().Version.ToString())
            .Enrich.WithProperty("api-gcsetting", GCSettings.IsServerGC ? "srv" : "wks")
            .Enrich.WithProperty("api-orchestrator", orchestrator.IsPresent() ? orchestrator : "notset")
            .Enrich.WithProperty("api-service-name", orchestratorService.IsPresent() ? orchestratorService : "notset");

            Log.Logger = loggerConfig.CreateLogger();
        }

        public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
        {
            readonly IApiVersionDescriptionProvider provider;
            public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
            {
                this.provider = provider;
            }

            public void Configure(SwaggerGenOptions options)
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                      description.GroupName,
                        new Microsoft.OpenApi.Models.OpenApiInfo()
                        {
                            Title = $"{description.GroupName} Syn Camera - {description.ApiVersion}",
                            Version = description.ApiVersion.ToString(),
                        });
                }
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddResponseCompression();
            services.AddHealthChecks();

            services.AddMvc(o =>
            {
                o.ValueProviderFactories.Insert(0, new SeparatedQueryStringValueProviderFactory(","));
            });

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
            services.AddVersionedApiExplorer(options => { options.GroupNameFormat = "'v'VVV"; options.SubstituteApiVersionInUrl = true; });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSingleton<IPostConfigureOptions<ApiKeyAuthenticationOptions>, ApiKeyAuthenticationOptionsPostConfigureOptions>();

            services.AddOptions();

            services.AddControllers();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = Constants.Security.ApiKeyAuthenticationScheme;
                o.DefaultSignOutScheme = Constants.Security.ApiKeyAuthenticationScheme;
                o.DefaultSignInScheme = Constants.Security.ApiKeyAuthenticationScheme;
                o.DefaultChallengeScheme = Constants.Security.ApiKeyAuthenticationScheme;
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Constants.Security.ApiKeyAuthenticationScheme, o =>
            {
                o.Realm = "SYN-SS-CAMERA";
                o.AllowTokenInQueryString = true;
                o.ReadKeyFromPath = false;
            });

            services.AddsynossCameraDefaults();

            services.AddAuthorization();

            services.AddMemoryCache();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("apiKey", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Client API key auhtorization. Example: \"Authorization: ApiKey {apiKey}\"",
                    Name = "Authorization",
                    Scheme = Constants.Security.ApiKeyAuthenticationScheme,
                    Type = SecuritySchemeType.Http
                });

                GetXmlCommentsPath().ForEach(xmlFile => c.IncludeXmlComments(xmlFile));
                //c.OperationFilter<AddBasicAuthorizationHeaderParameter>();
                c.ParameterFilter<QueryArrayParamFilter>();
            });

            //Setup CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllOrigins",
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin();
                                      builder.AllowAnyMethod();
                                      builder.AllowAnyHeader();
                                  });
            });
        }

        private List<string> GetXmlCommentsPath()
        {
            List<string> xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
            return xmlFiles;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // reduce traffic
            app.UseResponseCompression();
            // process time reporting
            app.UseProcessTimeReporting();
            // add /ping endpoint for orchestrated container’s ReadynessProbe
            app.UsePingEndpoint();
            // add /health endpoint for orchestrated container's LivnessProbe
            app.UseHealthChecks();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                        options.RoutePrefix = String.Empty;
                    }
                });

            app.UseCors("AllOrigins");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
            });

            //Redirect default version of Swagger
            app.UseWhen(context => !context.Request.Path.Value.Contains("/api/v"), appBuilder =>
            {

                var options = new RewriteOptions()
                    .AddRedirect("api/(.*)", "api/v1/$1", 302);
                appBuilder.UseRewriter(options);
            });
        }
    }
}
