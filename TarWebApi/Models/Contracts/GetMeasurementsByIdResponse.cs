namespace TarWebApi.Models.Contracts
{
    public class GetMeasurementsByIdResponse : GenericServiceResponse
    {
		public List<Measurement> Measurements { get; set; }	
	}
}
