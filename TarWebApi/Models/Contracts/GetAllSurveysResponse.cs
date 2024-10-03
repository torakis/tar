namespace TarWebApi.Models.Contracts
{
    public class GetAllSurveysResponse : GenericServiceResponse
    {
        public List<Survey> Surveys { get; set; }
    }
}
