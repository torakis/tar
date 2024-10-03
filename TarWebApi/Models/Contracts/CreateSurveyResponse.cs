namespace TarWebApi.Models.Contracts
{
    public class CreateSurveyResponse : GenericServiceResponse
    {
        public Survey Survey { get; set; }
    }
}
