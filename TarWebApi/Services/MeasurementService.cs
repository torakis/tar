﻿using System.Diagnostics.Metrics;
using System.Linq.Expressions;
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

            var projection = GetMeasurementProjection(request.MeasurementType, new ArgumentOutOfRangeException());

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

    private static ProjectionDefinition<Measurement, MeasurementProjection> GetMeasurementProjection(MeasurementType type, Exception argumentOutOfRangeException)
    {
        var projectionMap = new Dictionary<MeasurementType, Expression<Func<Measurement, MeasurementProjection>>>
        {
            { MeasurementType.Temperature, m => new MeasurementProjection { Date = m.Date, Value = m.Temperature } },
            { MeasurementType.Humidity, m => new MeasurementProjection { Date = m.Date, Value = m.Humidity } },
            { MeasurementType.Pressure, m => new MeasurementProjection { Date = m.Date, Value = m.Pressure } },
            { MeasurementType.WindSpeed, m => new MeasurementProjection { Date = m.Date, Value = m.WindSpeed } },
            { MeasurementType.WindDirection, m => new MeasurementProjection { Date = m.Date, Value = m.WindDirection } },
            { MeasurementType.Gust, m => new MeasurementProjection { Date = m.Date, Value = m.Gust } },
            { MeasurementType.Precipitation, m => new MeasurementProjection { Date = m.Date, Value = m.Precipitation } },
            { MeasurementType.UVI, m => new MeasurementProjection { Date = m.Date, Value = m.UVI } },
            { MeasurementType.Light, m => new MeasurementProjection { Date = m.Date, Value = m.Light } },
            { MeasurementType.Part03, m => new MeasurementProjection { Date = m.Date, Value = m.Part03 } },
            { MeasurementType.Part05, m => new MeasurementProjection { Date = m.Date, Value = m.Part05 } },
            { MeasurementType.Part10, m => new MeasurementProjection { Date = m.Date, Value = m.Part10 } },
            { MeasurementType.Part25, m => new MeasurementProjection { Date = m.Date, Value = m.Part25 } },
            { MeasurementType.Part50, m => new MeasurementProjection { Date = m.Date, Value = m.Part50 } },
            { MeasurementType.Part100, m => new MeasurementProjection { Date = m.Date, Value = m.Part100 } },
            { MeasurementType.PM10, m => new MeasurementProjection { Date = m.Date, Value = m.PM10 } },
            { MeasurementType.PM25, m => new MeasurementProjection { Date = m.Date, Value = m.PM25 } },
            { MeasurementType.PM100, m => new MeasurementProjection { Date = m.Date, Value = m.PM100 } },
            { MeasurementType.CO2, m => new MeasurementProjection { Date = m.Date, Value = m.CO2 } }
        };

        if (projectionMap.TryGetValue(type, out var projection))
        {
            return Builders<Measurement>.Projection.Expression(projection);
        }

        throw argumentOutOfRangeException;
    }

}
