using shcome.loxone.sensor.AppContext;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace shcome.loxone.sensor.Importer
{
    public class WindAlarmImporter : SingleReadingImporterBase
    {
        public WindAlarmImporter(SeonsorAppDbContext dbContext, ILoggerFactory loggerFactory, IMemoryCache cache) : base(dbContext, loggerFactory, cache)
        {
        }

        public override string ImporterType => "windalarm";
    }
}
