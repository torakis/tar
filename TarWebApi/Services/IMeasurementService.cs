using TarWebApi.Models;
using TarWebApi.Models.Contracts;

namespace TarWebApi.Services;

public interface IMeasurementService
{
    Task<GetMeasurementsByIdResponse> GetMeasurementsByIdAsync(GetMeasurementsByIdRequest request);
    Task<GetLastMeasurementByIdResponse> GetLastMeasurementByIdAsync(GetLastMeasurementByIdRequest request);
    Task<GetMeasurementsByPeriodResponse> GetMeasurementsByPeriodAsync(GetMeasurementsByPeriodRequest request);
    Task<GetPeriodStatisticsResponse> GetPeriodStatisticsAsync(GetPeriodStatisticsRequest request);
}

