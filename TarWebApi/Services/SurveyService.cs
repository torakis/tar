using MongoDB.Driver;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;

namespace TarWebApi.Services;

public class SurveyService : ISurveyService
{
    private readonly IMongoCollection<Survey> _surveysCollection;
    private readonly IMongoCollection<Survey> _surveyAnswersCollection;
    public SurveyService(IStationStoreDatabaseSettings settings, IMongoClient mongoClient)
    {
        var db = mongoClient.GetDatabase(settings.DatabaseName);
        _surveysCollection = db.GetCollection<Survey>(settings.SurveysCollectionName);
        _surveyAnswersCollection = db.GetCollection<Survey>(settings.SurveyAnswersCollectionName);
    }

    public async Task<GetAllSurveysResponse> GetAllSurveysAsync(GetAllSurveysRequest request)
    {
        var resp = new GetAllSurveysResponse() { IsSuccessful = true, ErrorText = "" };
        var surveys = await _surveysCollection.Find(_ => true).ToListAsync();
        if (surveys is null)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = $"No surveys found";
        }
        else
            resp.Surveys = surveys;
        return resp;
    }

    public async Task<CreateSurveyResponse> CreateSurveyAsync(CreateSurveyRequest request)
    {
        var resp = new CreateSurveyResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            await _surveysCollection.InsertOneAsync(request.Survey);
        }
        catch (Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.Message?.ToString();
        }
        return resp;
    }

    public async Task<SubmitSurveyAnswerResponse> SubmitSurveyAnswerAsync(SubmitSurveyAnswerRequest request)
    {
        var resp = new SubmitSurveyAnswerResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            await _surveyAnswersCollection.InsertOneAsync(request.Survey);
        }
        catch(Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.Message?.ToString();
        }
        return resp;
    }
}
