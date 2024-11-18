using shcome.loxone.sensor.AppContext;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace shcome.loxone.sensor.Importer
{
    public class PresenceDetectionImporter : SingleReadingImporterBase
    {
        public PresenceDetectionImporter(SeonsorAppDbContext dbContext, ILoggerFactory loggerFactory, IMemoryCache cache) : base(dbContext, loggerFactory, cache)
        {
        }

        public override string ImporterType => "praesenzerkennung";
    }
}
