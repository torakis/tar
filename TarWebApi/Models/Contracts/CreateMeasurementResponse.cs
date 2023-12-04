namespace TarWebApi.Models.Contracts
{
    public class CreateMeasurementResponse : GenericServiceResponse
    {
		public Measurement Measurement { get; set; }	
	}
}
