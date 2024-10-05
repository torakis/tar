using System;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;

namespace TarWebApi.Services;

public interface ISurveyService
{
    Task<GetAllSurveysResponse> GetAllSurveysAsync(GetAllSurveysRequest request);
    Task<GetAllSurveyAnswersResponse> GetAllSurveyAnswersAsync(GetAllSurveyAnswersRequest request);
    Task<CreateSurveyResponse> CreateSurveyAsync(CreateSurveyRequest request);
    Task<SubmitSurveyAnswerResponse> SubmitSurveyAnswerAsync(SubmitSurveyAnswerRequest request);
}

