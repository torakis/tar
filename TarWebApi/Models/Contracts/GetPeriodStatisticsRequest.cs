using TarWebApi.Models.Enum;

namespace TarWebApi.Models.Contracts
{
    public class GetPeriodStatisticsRequest
    {
        public string Id { get; set; }
        public Period Period { get; set; }
    }
}
