using System.Diagnostics.Metrics;
using Amazon.Runtime.Internal;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;
using TarWebApi.Models.Enum;

namespace TarWebApi.Services;

public class MeasurementService : IMeasurementService
{
    private readonly IMongoCollection<Measurement> _measurementsCollection;
    public MeasurementService(IStationStoreDatabaseSettings settings, IMongoClient mongoClient)
    {
        var db = mongoClient.GetDatabase(settings.DatabaseName);
        _measurementsCollection = db.GetCollection<Measurement>(settings.MeasurementsCollectionName);
    }

    public async Task<GetLastMeasurementByIdResponse> GetLastMeasurementByIdAsync(GetLastMeasurementByIdRequest request)
    {
        var resp = new GetLastMeasurementByIdResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            var lastMeasurement = await _measurementsCollection
                .Find(s => s.StationId == request.StationId)
                .SortByDescending(s => s.Date)
                .FirstOrDefaultAsync();
            if (lastMeasurement is null)
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Measurements for station with Id = {request.StationId} not found";
            }
            else
                resp.Measurement = lastMeasurement;
        }
        catch (Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.ToString();
        }
        return resp;
    }

    //for one measurement type
    public async Task<GetMeasurementByPeriodResponse> GetMeasurementByPeriodAsync(GetMeasurementByPeriodRequest request)
    {
        var resp = new GetMeasurementByPeriodResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            var filter = Builders<Measurement>.Filter.Where(s => s.StationId == request.StationId
                && s.Date >= request.DateFrom
                && s.Date <= request.DateTo);

            // Define the projection based on the requested MeasurementType
            ProjectionDefinition<Measurement, MeasurementProjection> projection;

            switch (request.MeasurementType)
            {
                case MeasurementType.Temperature:
                    projection = Builders<Measurement>.Projection.Expression(m => new MeasurementProjection
                    {
                        Date = m.Date,
                        Value = m.Temperature
                    });
                    break;

                case MeasurementType.Humidity:
                    projection = Builders<Measurement>.Projection.Expression(m => new MeasurementProjection
                    {
                        Date = m.Date,
                        Value = m.Humidity
                    });
                    break;

                case MeasurementType.Pressure:
                    projection = Builders<Measurement>.Projection.Expression(m => new MeasurementProjection
                    {
                        Date = m.Date,
                        Value = m.Pressure
                    });
                    break;

                case MeasurementType.WindSpeed:
                    projection = Builders<Measurement>.Projection.Expression(m => new MeasurementProjection
                    {
                        Date = m.Date,
                        Value = m.WindSpeed
                    });
                    break;

                // Add cases for all other MeasurementType fields here

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var measurements = await _measurementsCollection
                .Find(filter)
                .Project(projection)
                .ToListAsync();

            if (measurements == null || !measurements.Any())
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Measurements for station {request.StationId} not found for the requested period.";
            }
            else
            {
                resp.Measurements = measurements;
            }
        }
        catch (Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.ToString();
        }
        return resp;
    }

    //Gets the measurements for specific station for the requested period
    //A valid day is from 00:00 - 23:59
    //calculate the average depending on period
    //for day --> group by 1 hour = 24 values
    //for week --> group by 4 hours = 42 values
    //for month --> group by 1 day = 30 values

    public async Task<GetMeasurementsByPeriodResponse> GetMeasurementsByPeriodAsync(GetMeasurementsByPeriodRequest request)
    {
        var resp = new GetMeasurementsByPeriodResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            var measurements = await _measurementsCollection.Find(s => s.StationId == request.StationId && s.Date >= request.DateFrom).ToListAsync();
            if (measurements is null)
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Measurements for station {request.StationId} not found for the requested period.";
            }
            else
                resp.Measurements = measurements;
        }
        catch (Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.ToString();
        }
        return resp;
    }
}
