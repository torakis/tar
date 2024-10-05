using TarWebApi.Models;
using TarWebApi.Models.Contracts;

namespace TarWebApi.Services;

public interface IMeasurementService
{
    Task<GetLastMeasurementByIdResponse> GetLastMeasurementByIdAsync(GetLastMeasurementByIdRequest request);
    Task<GetMeasurementByPeriodResponse> GetMeasurementByPeriodAsync(GetMeasurementByPeriodRequest request);
    Task<GetMeasurementsByPeriodResponse> GetMeasurementsByPeriodAsync(GetMeasurementsByPeriodRequest request);
}

