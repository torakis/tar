namespace TarWebApi.Models.Contracts
{
    public class SubmitSurveyAnswerRequest
    {
        public Survey Survey { get; set; }
        public string DeviceId { get; set; }
    }
}
