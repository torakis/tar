namespace TarWebApi.Models.Contracts
{
    public class GetMeasurementByPeriodResponse : GenericServiceResponse
    {
		public List<MeasurementProjection> Measurements { get; set; }	
	}
}
