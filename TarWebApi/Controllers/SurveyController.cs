using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;
using TarWebApi.Services;

namespace TarWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurveyController : ControllerBase
{
    private readonly ISurveyService _surveyService;
    public SurveyController(ISurveyService surveyService) =>
        _surveyService = surveyService;
    
    [HttpPost]
    [Route("GetAllSurveys")]
    public async Task<ActionResult<GetAllSurveysResponse>> GetAllSurveys(GetAllSurveysRequest request)
    {
        var response = await _surveyService.GetAllSurveysAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("CreateSurvey")]
    public async Task<ActionResult<CreateSurveyResponse>> CreateSurvey(CreateSurveyRequest request)
    {
        var response = await _surveyService.CreateSurveyAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("SubmitSurveyAnswer")]
    public async Task<ActionResult<SubmitSurveyAnswerResponse>> SubmitSurveyAnswer(SubmitSurveyAnswerRequest request)
    {
        var response = await _surveyService.SubmitSurveyAnswerAsync(request);        
        return Ok(response);
    }
}

