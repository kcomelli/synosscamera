using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using shcome.core.Diagnostics;
using shcome.core.Infrastructure.Http;
using shcome.station.Abstractions;
using shcome.station.Api;
using shcome.station.Configuration;
using shcome.station.Infrastructure;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace shcome.station.DependencyInjection
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Surveillance station apis
        /// </summary>
        /// <param name="services">Service collection instance</param>
        /// <param name="configuration">Configuration access</param>
        /// <returns>Service collection instance</returns>
        public static IServiceCollection AddSurveillanceStationApis(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckArgumentNull(nameof(services));

            services.AddOptions();
            services.Configure<SurveillanceStationSettings>(configuration.GetSection(nameof(SurveillanceStationSettings)));

            services.AddScoped<SurveillanceStationClient>();

            services.AddHttpClient(typeof(SurveillanceStationClient).Name)
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        return HttpClientHandlerBuilder.Create()
                                .SetAllowAutoRedirect(true)
                                .SetAutoDecompression(DecompressionMethods.Deflate | DecompressionMethods.GZip)
                                .SetUseDefaultCredentials(true)
                                .SetAcceptAllCertificates()
                                .Build();
                    })
                    .ConfigureHttpClient(client =>
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                        //client.BaseAddress = new Uri("");
                    })
                    // add retrying policy for all methods except POST
                    .AddPolicyHandler(request => request.Method != HttpMethod.Post ? RetryPolicy() : NoOpPolicy())
                    // block clients after 15 errors for 30seconds to avoid unwanted DoS attacks
                    .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(15, TimeSpan.FromSeconds(30))); ;

            services.AddApis();

            return services;
        }

        private static IServiceCollection AddApis(this IServiceCollection services)
        {

            services.AddScoped<IStationApiUtil, DefaultStationApiUtil>();

            services.AddScoped<ApiInfo>();
            services.AddScoped<ApiAuth>();
            services.AddScoped<ApiCamera>();
            services.AddScoped<ApiExternalRecording>();
            services.AddScoped<ApiHomeMode>();

            // add apis with interface also - this is for util to update/sync tokens
            services.AddScoped<IStationApi, ApiInfo>();
            services.AddScoped<IStationApi, ApiAuth>();
            services.AddScoped<IStationApi, ApiCamera>();
            services.AddScoped<IStationApi, ApiExternalRecording>();
            services.AddScoped<IStationApi, ApiHomeMode>();

            return services;
        }

        /// <summary>
        /// No operation policy
        /// </summary>
        /// <returns></returns>
        private static IAsyncPolicy<HttpResponseMessage> NoOpPolicy()
        {
            return Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();
        }
        /// <summary>
        /// Retry policy with 3 retries and 3,9,27 seconds back-off
        /// </summary>
        /// <returns></returns>
        private static IAsyncPolicy<HttpResponseMessage> RetryPolicy()
        {
            return HttpPolicyExtensions
                      .HandleTransientHttpError() // on 5xx and 408 codes
                      .WaitAndRetryAsync(3, (retryNr) => TimeSpan.FromSeconds(Math.Pow(3, retryNr))); // wait 3, 9, 27 seconds before next call (back-off)
        }
    }
}
