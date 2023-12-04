namespace TarWebApi.Models.Contracts
{
    public class GetMeasurementsByPeriodResponse : GenericServiceResponse
    {
		public List<Measurement> Measurements { get; set; }	
	}
}
