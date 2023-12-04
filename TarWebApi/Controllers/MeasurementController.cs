using Microsoft.AspNetCore.Mvc;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;
using TarWebApi.Services;

namespace TarWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementController : ControllerBase
{
    private readonly IMeasurementService _measurementService;
    public MeasurementController(IMeasurementService measurementService) =>
        _measurementService = measurementService;

    [HttpPost]
    [Route("CreateMeasurement")]
    public async Task<ActionResult<CreateMeasurementResponse>> CreateMeasurement(CreateMeasurementRequest request)
    {
        var response = await _measurementService.CreateMeasurementAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("GetMeasurementsById")]
    public async Task<ActionResult<GetMeasurementsByIdResponse>> GetMeasurementsById(GetMeasurementsByIdRequest request)
    {
        var response = await _measurementService.GetMeasurementsByIdAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("GetMeasurementsByPeriod")]
    public async Task<ActionResult<GetMeasurementsByPeriodResponse>> GetMeasurementsByPeriod(GetMeasurementsByPeriodRequest request)
    {
        var response = await _measurementService.GetMeasurementsByPeriodAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("GetPeriodStatistics")]
    public async Task<ActionResult<GetPeriodStatisticsResponse>> GetPeriodStatistics(GetPeriodStatisticsRequest request)
    {
        var response = await _measurementService.GetPeriodStatisticsAsync(request);
        return Ok(response);
    }
}
