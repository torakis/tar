using TarWebApi.Models.Enum;

namespace TarWebApi.Models.Contracts
{
    public class GetMeasurementsByPeriodRequest
    {
        public string Id { get; set; }
        public Period Period { get; set; }
    }
}
