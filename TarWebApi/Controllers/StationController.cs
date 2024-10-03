using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;
using TarWebApi.Services;

namespace TarWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StationController : ControllerBase
{
    private readonly IStationService _stationService;
    public StationController(IStationService stationService) =>
        _stationService = stationService;
    
    [HttpPost]
    [Route("GetAllStations")]
    public async Task<ActionResult<GetAllStationsResponse>> GetAllStations(GetAllStationsRequest request)
    {
        var response = await _stationService.GetAllStationsAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("GetStationById")]
    public async Task<ActionResult<GetStationByIdResponse>> GetStationById(GetStationByIdRequest request)
    {
        var response = await _stationService.GetStationByIdAsync(request);        
        return Ok(response);
    }

    [HttpPost]
    [Route("CreateStation")]
    public async Task<ActionResult<CreateStationResponse>> CreateStation(CreateStationRequest request)
    {
        var response = await _stationService.CreateStationAsync(request);
        return Ok(response);
    }
}

