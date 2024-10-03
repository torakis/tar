using TarWebApi.Models.Enum;

namespace TarWebApi.Models.Contracts
{
    public class GetMeasurementsByPeriodRequest
    {
        public required string StationId { get; set; }
        public Period Period { get; set; } = Period.Day;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
