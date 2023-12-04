namespace TarWebApi.Models.Contracts
{
    public class GetAllStationsResponse : GenericServiceResponse
    {
		public List<Station> Stations { get; set; }	
	}
}
