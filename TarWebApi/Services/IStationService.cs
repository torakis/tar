using System;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;

namespace TarWebApi.Services;

public interface IStationService
{
    Task<GetAllStationsResponse> GetAllStationsAsync(GetAllStationsRequest request);
    Task<GetStationByIdResponse> GetStationByIdAsync(GetStationByIdRequest request);
    Task<CreateStationResponse> CreateStationAsync(CreateStationRequest request);
}

