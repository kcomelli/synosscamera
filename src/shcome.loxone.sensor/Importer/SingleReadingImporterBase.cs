using shcome.loxone.sensor.AppContext;
using shcome.loxone.sensor.Clients;
using shcome.loxone.sensor.Config;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shcome.loxone.sensor.Importer
{
    public abstract class SingleReadingImporterBase : ImporterBase
    {
        protected SingleReadingImporterBase(SeonsorAppDbContext dbContext, ILoggerFactory loggerFactory, IMemoryCache cache) : base(dbContext, loggerFactory, cache)
        {
        }

        public virtual string ReadingDescriminator => "aktuell";

        public override async Task ImportFile(string fileName, string sensorName = null, CancellationToken cancellationToken = default)
        {
            FileInfo fi = new FileInfo(fileName);
            Logger.LogInformation("Start importing file '{fileName}'.", fi.Name);

            var sensor = await ResolveSensor(fi.Name, sensorName, cancellationToken);
            if (sensor != null)
            {
                Logger.LogInformation("Resolved sensor with id '{sensorId}'.", sensor);
                var alltext = File.ReadAllLines(fileName);
                var latestEntry = await LatestReading(sensor, cancellationToken);
                var cnt = 0;
                var dataAdded = 0;
                foreach (var csvLine in alltext)
                {
                    if (cnt > 0)
                    {
                        var values = csvLine.Split(new char[] { ';' });
                        var deCulture = new CultureInfo("de-AT");

                        var dateTimeTest = $"{values[0]} {values[1]}";
                        if (DateTime.TryParseExact(dateTimeTest, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTimestamp))
                        {
                            if (latestEntry == default || parsedTimestamp > latestEntry)
                            {
                                var reading = double.Parse(values[2], deCulture.NumberFormat);

                                Logger.LogInformation("Adding sensor value: temperature='{temperature}'.", reading);

                                await InsertReading(sensor, ReadingDescriminator, parsedTimestamp, reading, cancellationToken);
                                dataAdded++;
                            }
                            else
                            {
                                Logger.LogWarning("Skipping line of timestamp '{timestamp}' because data with a later date already imported!", dateTimeTest);
                            }
                        }
                        else
                        {
                            Logger.LogError("Could not parse timestamp '{timestamp}'. Skipping line.", dateTimeTest);
                        }
                    }
                    cnt++;
                }

                if (dataAdded > 0)
                {
                    Logger.LogInformation("Persisting {numberOfRecords} number of new records.", dataAdded);
                    await SaveChangesAsync(cancellationToken);
                }

                Logger.LogInformation("Finished importing file '{fileName}'.", fi.Name);
            }
            else
            {
                Logger.LogError("Could not resolve sensor using sensor name '{sensorName}' and filename '{fileName}'.", sensorName, fi.Name);
            }
        }

        public override async Task ImportFromEndpoint(SensorEndpoint endpoint, string sensorName, CancellationToken cancellationToken = default)
        {
            var client = new LoxoneStatsClient(endpoint, Logger);

            var availableSensorData = await client.LoadSensorInformation(cancellationToken);
            if (availableSensorData?.Count > 0)
            {
                Logger.LogInformation("Found sensor data checking relevancy");
                Logger.LogDebug("Loading all sensors and their latest readings.");
                var sensors = await GetSensors(cancellationToken);
                var latestReadings = await GetLatestReadings(cancellationToken);

                Logger.LogDebug("Loaded {numberOfSensors} number of sensors.", sensors.Count);
                var dataAdded = 0;

                foreach (var stats in availableSensorData)
                {
                    if (!string.IsNullOrEmpty(sensorName) && sensorName != stats.SensorName)
                    {
                        Logger.LogInformation("Skipping sensor '{sensorName}' because explicit sensor import for '{importSensorName}' was requested!", stats.SensorName, sensorName);
                        continue;
                    }

                    var descr = ToSensorDescriminator(stats.Category);

                    if (descr != ImporterType)
                    {
                        Logger.LogDebug("Skipping sensor {sensorDescriminator} because it does not match this importer type '{importerType}'.", ImporterType);
                        continue;
                    }

                    var sensor = sensors.FirstOrDefault(s => s.Name == stats.SensorName && s.Room == stats.Room);
                    var latestReading = (DateTime?)null;

                    if (sensor == null)
                    {
                        sensor = await CreateSensor(stats.SensorName, stats.Description, descr, stats.Category, stats.Room, cancellationToken);
                        sensors.Add(sensor);
                    }
                    else
                    {
                        latestReading = latestReadings.FirstOrDefault(r => r.Key == sensor.Id).Value;
                    }

                    if (!latestReading.HasValue || !stats.ReadingTimeSpanAsDateTime.HasValue ||
                        (latestReading.Value.Year < stats.ReadingTimeSpanAsDateTime.Value.Year || (latestReading.Value.Year == stats.ReadingTimeSpanAsDateTime.Value.Year && latestReading.Value.Month <= stats.ReadingTimeSpanAsDateTime.Value.Month)))
                    {
                        // import readings from file
                        var sensorReadings = await client.LoadSensorReadings(stats, cancellationToken);

                        foreach (var sensorData in sensorReadings.Rows)
                        {
                            if (!latestReading.HasValue || latestReading.Value < sensorData.Timestamp)
                            {
                                Logger.LogDebug("Adding sensor value: temperature='{temperature}'.", sensorData.Value);
                                await InsertReading(sensor, ReadingDescriminator, sensorData.Timestamp ?? default(DateTime), sensorData.Value, cancellationToken);
                                dataAdded++;
                            }
                        }
                    }
                }

                if (dataAdded > 0)
                {
                    Logger.LogInformation("Persisting {numberOfRecords} number of new records.", dataAdded);
                    await SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
