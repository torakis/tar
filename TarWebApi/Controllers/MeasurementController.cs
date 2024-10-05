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
    [Route("GetLastMeasurementById")]
    public async Task<ActionResult<GetLastMeasurementByIdResponse>> GetLastMeasurementById(GetLastMeasurementByIdRequest request)
    {
        var response = await _measurementService.GetLastMeasurementByIdAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("GetMeasurementByPeriod")]
    public async Task<ActionResult<GetMeasurementByPeriodResponse>> GetMeasurementByPeriod(GetMeasurementByPeriodRequest request)
    {
        var response = await _measurementService.GetMeasurementByPeriodAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("GetMeasurementsByPeriod")]
    public async Task<ActionResult<GetMeasurementsByPeriodResponse>> GetMeasurementsByPeriod(GetMeasurementsByPeriodRequest request)
    {
        var response = await _measurementService.GetMeasurementsByPeriodAsync(request);
        return Ok(response);
    }
}
