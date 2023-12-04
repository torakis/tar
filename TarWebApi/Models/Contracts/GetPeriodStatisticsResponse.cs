namespace TarWebApi.Models.Contracts
{
    public class GetPeriodStatisticsResponse : GenericServiceResponse
    {
		public List<Statistics> Statistics { get; set; }	
	}
}
