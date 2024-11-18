using shcome.loxone.sensor.AppContext;
using shcome.loxone.sensor.Clients;
using shcome.loxone.sensor.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace shcome.loxone.sensor.Importer
{
    public abstract class ImporterBase
    {
        private readonly SeonsorAppDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly List<EntityObjects.SensorValue> addedSensorReadings = new List<EntityObjects.SensorValue>();

        protected ImporterBase(SeonsorAppDbContext dbContext, ILoggerFactory loggerFactory, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _logger = loggerFactory.CreateLogger(GetType().FullName);
            _memoryCache = cache;
        }

        public abstract string ImporterType { get; }
        protected ILogger Logger => _logger;

        public async Task<List<Model.StatsSensorData>> LoadSensorInformation(LoxoneStatsClient client, CancellationToken cancellationToken = default)
        {
            var cacheKey = client.Endpoint.EndpointUrl;

            if(_memoryCache.TryGetValue<List<Model.StatsSensorData>>(cacheKey, out List<Model.StatsSensorData> cachedData))
            {
                return cachedData;
            }
            var fetchedData = await client.LoadSensorInformation(cancellationToken);
            if(fetchedData != null)
            {
                _memoryCache.Set(cacheKey, fetchedData);
            }

            return fetchedData;
        }
        public abstract Task ImportFile(string fileName, string sensorName = null, CancellationToken cancellationToken = default);
        public abstract Task ImportFromEndpoint(SensorEndpoint endpoint, string sensorName, CancellationToken cancellationToken = default);

        protected async Task<int?> ResolveSensorId(string fileName, string sensorName, CancellationToken cancellationToken = default)
        {
            int sensorId = 0;
            if (!string.IsNullOrEmpty(sensorName))
            {
                sensorId = await FindSensorId(sensorName, cancellationToken);
                if (sensorId > 0)
                    return sensorId;
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                var sensorFromFile = fileName.Split(new char[] { '-' });
                sensorName = sensorFromFile.First().Trim();
                sensorId = await FindSensorId(sensorName, cancellationToken);
                return sensorId > 0 ? sensorId : (int?)null;
            }

            return null;
        }

        protected async Task<EntityObjects.Sensor> ResolveSensor(string fileName, string sensorName, CancellationToken cancellationToken = default)
        {
            int sensorId = 0;
            if (!string.IsNullOrEmpty(sensorName))
            {
                return await FindSensor(sensorName, cancellationToken);
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                var sensorFromFile = fileName.Split(new char[] { '-' });
                sensorName = sensorFromFile.First().Trim();
                return await FindSensor(sensorName, cancellationToken);
            }

            return null;
        }

        protected Task<int> FindSensorId(string sensorName, CancellationToken cancellationToken = default)
        {
            return _dbContext.sensor.Where(s => s.Name.Contains(sensorName))
                                    .Select(s => s.Id)
                                    .FirstOrDefaultAsync(cancellationToken);
        }

        protected Task<EntityObjects.Sensor> FindSensor(string sensorName, CancellationToken cancellationToken = default)
        {
            return _dbContext.sensor.Where(s => s.Name.Contains(sensorName))
                                    .FirstOrDefaultAsync(cancellationToken);
        }

        protected Task<DateTime> LatestReading(int sensorId, CancellationToken cancellationToken = default)
        {
            return _dbContext.sensor_value.Where(s => s.SensorId == sensorId)
                        .OrderByDescending(s => s.Timestamp)
                        .Select(s => s.Timestamp)
                        .FirstOrDefaultAsync(cancellationToken);
        }

        protected Task<DateTime> LatestReading(EntityObjects.Sensor sensor, CancellationToken cancellationToken = default)
        {
            if (sensor == null || sensor.Id == 0)
                return Task.FromResult(default(DateTime));

            return _dbContext.sensor_value.Where(s => s.SensorId == sensor.Id)
                        .OrderByDescending(s => s.Timestamp)
                        .Select(s => s.Timestamp)
                        .FirstOrDefaultAsync(cancellationToken);
        }

        protected Task<List<EntityObjects.Sensor>> GetSensors(CancellationToken cancellationToken = default)
        {
            return _dbContext.sensor.ToListAsync(cancellationToken);
        }

        protected Task<Dictionary<int, DateTime?>> GetLatestReadings(CancellationToken cancellationToken = default)
        {
            return _dbContext.sensor_value.GroupBy(k => k.SensorId)
                        .Select(g => new
                        {
                            SensorId = g.Key,
                            LatestReading = g.Max(r => r.Timestamp as DateTime?)
                        }).ToDictionaryAsync(k => k.SensorId, v => v.LatestReading, cancellationToken);
        }

        protected Task InsertReading(EntityObjects.Sensor sensor, string descriminator, DateTime timestamp, double sensorValue, CancellationToken cancellationToken = default)
        {
            
            var newSensorReading = new EntityObjects.SensorValue()
            {
                Descriminator = descriminator,
                Sensor = sensor,
                Timestamp = timestamp,
                Reading = sensorValue
            };

            if (addedSensorReadings.Any(s => s.Equals(newSensorReading)))
            {
                // skip adding values because a value for this sensor, descriminatr and timestamp with the given reading already exists
                return Task.CompletedTask;
            }
            _dbContext.sensor_value.Add(newSensorReading);
            addedSensorReadings.Add(newSensorReading);

            return Task.CompletedTask;
        }

        protected Task<EntityObjects.Sensor> CreateSensor(string name, string description, string type, string category, string room, CancellationToken cancellationToken = default)
        {
            var newSensor = new EntityObjects.Sensor()
            {
                Name = name,
                Description = description,
                Type = type,
                Category = category,
                Room = room
            };


            _dbContext.sensor.Add(newSensor);

            return Task.FromResult(newSensor);
        }

        protected string ToSensorDescriminator(string input)
        {
            return input.ToLower().Replace("ä", "ae").Replace("ü", "ue").Replace("ö", "oe");
        }

        protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                addedSensorReadings.Clear();
            }
        }
    }
}
