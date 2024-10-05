namespace TarWebApi.Models.Contracts
{
    public class GetAllSurveyAnswersResponse : GenericServiceResponse
    {
        public List<SurveyAnswer> SurveyAnswers { get; set; }
    }
}
