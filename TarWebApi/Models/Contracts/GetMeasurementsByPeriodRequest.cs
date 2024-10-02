using TarWebApi.Models.Enum;

namespace TarWebApi.Models.Contracts
{
    public class GetMeasurementsByPeriodRequest
    {
        public required string StationId { get; set; }
        public Period Period { get; set; } = Period.Day;
    }
}
