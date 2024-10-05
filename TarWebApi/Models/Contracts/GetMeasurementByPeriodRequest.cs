using TarWebApi.Models.Enum;

namespace TarWebApi.Models.Contracts
{
    public class GetMeasurementByPeriodRequest
    {
        public required string StationId { get; set; }
        public required MeasurementType MeasurementType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
