using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    public class ImportBackgroundWorker<TImporter> : StartupCoordinatedBackgroundService
        where TImporter: ImporterBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SensorOptions _sensorOptions;

        public ImportBackgroundWorker(IServiceProvider serviceProvider, 
                                      IOptions<SensorOptions> options,
                                        IHostApplicationLifetime appLifeTime, 
                                        ILoggerFactory loggerFactory)
            :base(appLifeTime, loggerFactory)
        {
            serviceProvider.CheckArgumentNull(nameof(serviceProvider));


            _serviceProvider = serviceProvider;
            _sensorOptions = options?.Value ?? new SensorOptions();
        }

        protected IServiceProvider ServiceProvider => _serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            while (cancellationToken.IsCancellationRequested)
            {
                using (var sp = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    Console.WriteLine($"Starting {typeof(TImporter).Name} import ...");
                    try
                    {
                        var importer = sp.ServiceProvider.GetRequiredService<TImporter>();

                        foreach (var endpoint in _sensorOptions.Endpoints)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            try
                            {
                                Console.WriteLine($"Importing readings for {typeof(TImporter).Name} from endpoint: " + endpoint.Name);
                                await importer.ImportFromEndpoint(endpoint, null, cancellationToken);
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine($"Error importing readings for {typeof(TImporter).Name} from endpoint '" + endpoint.Name + "': " + ex);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error {typeof(TImporter).Name}: " + ex);
                    }
                    finally
                    {
                        Console.WriteLine($"Import {typeof(TImporter).Name} completed!");
                    }
                }

                await Task.Delay(_sensorOptions.DefaultImportFrequencyInMs, cancellationToken);
            }
        }
    }
}
