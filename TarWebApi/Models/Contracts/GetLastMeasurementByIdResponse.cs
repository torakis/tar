namespace TarWebApi.Models.Contracts
{
    public class GetLastMeasurementByIdResponse : GenericServiceResponse
    {
		public Measurement Measurement { get; set; }	
	}
}
