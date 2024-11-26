using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using shcome.core.Abstractions;
using shcome.core.Diagnostics;
using shcome.core.Infrastructure.Hosting;
using shcome.loxone.sensor.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shcome.loxone.sensor.Importer
{
    /// <summary>
    /// Background hosted service responsible for importing loxone sensor data
    /// </summary>
    /// <typeparam name="TImporter">Importer implementation class</typeparam>
    public class ImportBackgroundWorker<TImporter> : StartupCoordinatedBackgroundService
        where TImporter: ImporterBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SensorOptions _sensorOptions;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="serviceProvider">Service provider instance.</param>
        /// <param name="options">Sensor options value.</param>
        /// <param name="appLifeTime">Application lifetime events.</param>
        /// <param name="loggerFactory">Logger factory instance.</param>
        /// <param name="logEnricher">Log enricher instance.</param>
        public ImportBackgroundWorker(IServiceProvider serviceProvider, 
                                        IOptions<SensorOptions> options,
                                        IHostApplicationLifetime appLifeTime, 
                                        ILoggerFactory loggerFactory,
                                        ILogEnricher logEnricher)
            :base(appLifeTime, loggerFactory, logEnricher)
        {
            serviceProvider.CheckArgumentNull(nameof(serviceProvider));


            _serviceProvider = serviceProvider;
            _sensorOptions = options?.Value ?? new SensorOptions();
        }
        /// <summary>
        /// Get the service provider instance
        /// </summary>
        protected IServiceProvider ServiceProvider => _serviceProvider;
        /// <summary>
        /// Get the importer name
        /// </summary>
        public string Importer => typeof(TImporter).Name;
        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            EnrichLogger("Importer", Importer);

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var sp = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    //Console.WriteLine($"Starting {typeof(TImporter).Name} import ...");
                    Logger.LogInformation("Starting import ...");
                    try
                    {
                        var importer = sp.ServiceProvider.GetRequiredService<TImporter>();

                        foreach (var endpoint in _sensorOptions.Endpoints)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            try
                            {
                                Logger.LogDebug("Importing readings from endpoint: {endpoint}", endpoint.Name);
                                //Console.WriteLine($"Importing readings for {typeof(TImporter).Name} from endpoint: " + endpoint.Name);
                                await importer.ImportFromEndpoint(endpoint, null, cancellationToken);
                            }
                            catch(Exception ex)
                            {
                                //Console.WriteLine($"Error importing readings for {typeof(TImporter).Name} from endpoint '" + endpoint.Name + "': " + ex);
                                Logger.LogError(ex, "Error importing readings readings from endpoint '{endpoint}': {error}", endpoint.Name, ex.Message);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        //Console.WriteLine($"Error {typeof(TImporter).Name}: " + ex);
                        Logger.LogError(ex, "Unhandled error: {error} (import completed with errors)", ex.Message);
                    }
                    finally
                    {
                        //Console.WriteLine($"Import {typeof(TImporter).Name} completed!");
                        Logger.LogInformation("Import completed!");
                    }
                }

                try
                {
                    await Task.Delay(_sensorOptions.DefaultImportFrequencyInMs, cancellationToken);
                }
                catch(OperationCanceledException)
                {
                    //Console.WriteLine($"Shutting down {typeof(TImporter).Name}.");
                    Logger.LogInformation("Shutting down import worker!");
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"Error while importer {typeof(TImporter).Name} was in wait state: " + ex);
                    Logger.LogError(ex, "Unhandled error while importer was in wait state: {error} (worker is shutting down)", ex.Message);
                }
            }
        }
    }
}
