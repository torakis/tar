namespace TarWebApi.Models.Contracts
{
    public class GetStationByIdResponse: GenericServiceResponse
    {
		public Station? Station { get; set; }	
	}
}
