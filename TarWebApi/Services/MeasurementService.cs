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

    //Gets the measurements for specific station for the requested period, for one measurement type
    //A valid day is from 00:00 - 23:59
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
                resp.Measurements = CalculateAverageForMeasurementProjection(measurements, request.DateFrom, request.DateTo);
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
    public async Task<GetMeasurementsByPeriodResponse> GetMeasurementsByPeriodAsync(GetMeasurementsByPeriodRequest request)
    {
        var resp = new GetMeasurementsByPeriodResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            var filter = Builders<Measurement>.Filter.Where(s => s.StationId == request.StationId
                && s.Date >= request.DateFrom
                && s.Date <= request.DateTo);

            var measurements = await _measurementsCollection
                .Find(filter)
                .ToListAsync();

            if (measurements == null || !measurements.Any())
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Measurements for station {request.StationId} not found for the requested period.";
            }
            else
            {
                resp.Measurements = CalculateAverageForMeasurements(measurements, request.DateFrom, request.DateTo);
            }
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

    private static List<MeasurementProjection> CalculateAverageForMeasurementProjection(List<MeasurementProjection> measurements, DateTime? dateFrom, DateTime? dateTo)
    {
        var resp = new List<MeasurementProjection>();

        try
        {
            var dateDiff = (dateTo - dateFrom)?.TotalDays ?? 0;

            // Determine the grouping interval based on the date difference
            TimeSpan interval;
            if (dateDiff < 1)
            {
                interval = TimeSpan.FromHours(1);  // Group by hour if the date range is less than 1 day
            }
            else if (dateDiff >= 1 && dateDiff <= 7)
            {
                interval = TimeSpan.FromHours(4);  // Group by 4 hours if the date range is between 1 and 7 days
            }
            else
            {
                interval = TimeSpan.FromDays(1);  // Group by day if the date range is greater than 7 days
            }

            // Group measurements based on the interval
            var groupedMeasurements = measurements
                .GroupBy(m =>
                {
                    if (interval == TimeSpan.FromDays(1))
                    {
                        // If grouping by day, return only the year, month, and day (ignore the time part)
                        return new DateTime(m.Date.Year, m.Date.Month, m.Date.Day);
                    }
                    else if (interval == TimeSpan.FromHours(4))
                    {
                        // Adjust for 4-hour grouping
                        return new DateTime(m.Date.Year, m.Date.Month, m.Date.Day, (m.Date.Hour / 4) * 4, 0, 0);
                    }
                    else
                    {
                        // Group by hour
                        return new DateTime(m.Date.Year, m.Date.Month, m.Date.Day, m.Date.Hour, 0, 0);
                    }
                })
                .ToList();

            // Calculate the average for each group
            foreach (var group in groupedMeasurements)
            {
                var averageValue = group.Average(m => m.Value);  // Calculate average of the "Value"
                averageValue = averageValue != null ? Math.Round(averageValue.Value, 2) : null;
                var groupDate = group.Key;  // The key is the date of the group (with or without time)

                // Create a new MeasurementProjection for the averaged data
                resp.Add(new MeasurementProjection
                {
                    Date = groupDate,  // Use the grouped date
                    Value = averageValue  // Set the average value for the group
                });
            }
        }
        catch (Exception ex)
        {
            // Optionally handle the exception (for logging or debugging)
            Console.WriteLine(ex.Message);
        }

        return resp;
    }

    private static List<Measurement> CalculateAverageForMeasurements(List<Measurement> measurements, DateTime? dateFrom, DateTime? dateTo)
    {
        var resp = new List<Measurement>();

        try
        {
            var dateDiff = (dateTo - dateFrom)?.TotalDays ?? 0;

            // Determine the grouping interval based on the date difference
            TimeSpan interval;
            if (dateDiff < 1)
            {
                interval = TimeSpan.FromHours(1);  // Group by hour if the date range is less than 1 day
            }
            else if (dateDiff >= 1 && dateDiff <= 7)
            {
                interval = TimeSpan.FromHours(4);  // Group by 4 hours if the date range is between 1 and 7 days
            }
            else
            {
                interval = TimeSpan.FromDays(1);  // Group by day if the date range is greater than 7 days
            }

            // Group measurements based on the interval
            var groupedMeasurements = measurements
                .GroupBy(m =>
                {
                    if (interval == TimeSpan.FromDays(1))
                    {
                        // If grouping by day, return only the year, month, and day (ignore the time part)
                        return new DateTime(m.Date.Year, m.Date.Month, m.Date.Day);
                    }
                    else if (interval == TimeSpan.FromHours(4))
                    {
                        // Adjust for 4-hour grouping
                        return new DateTime(m.Date.Year, m.Date.Month, m.Date.Day, (m.Date.Hour / 4) * 4, 0, 0);
                    }
                    else
                    {
                        // Group by hour
                        return new DateTime(m.Date.Year, m.Date.Month, m.Date.Day, m.Date.Hour, 0, 0);
                    }
                })
                .ToList();

            // Calculate the average for each group
            foreach (var group in groupedMeasurements)
            {
                var groupDate = group.Key;  // The key is the date of the group (with or without time)

                // Create a new Measurement for the averaged data
                var averagedMeasurement = new Measurement
                {
                    Date = groupDate,  // Use the grouped date

                    // Calculate average for all the fields
                    Temperature = group.Average(m => m.Temperature) != null ? Math.Round(group.Average(m => m.Temperature).Value, 2) : null,
                    Humidity = group.Average(m => m.Humidity) != null ? Math.Round(group.Average(m => m.Humidity).Value, 2) : null,
                    Pressure = group.Average(m => m.Pressure) != null ? Math.Round(group.Average(m => m.Pressure).Value, 2) : null,
                    WindSpeed = group.Average(m => m.WindSpeed) != null ? Math.Round(group.Average(m => m.WindSpeed).Value, 2) : null,
                    WindDirection = group.Average(m => m.WindDirection) != null ? Math.Round(group.Average(m => m.WindDirection).Value, 2) : null,
                    Gust = group.Average(m => m.Gust) != null ? Math.Round(group.Average(m => m.Gust).Value, 2) : null,
                    Precipitation = group.Average(m => m.Precipitation) != null ? Math.Round(group.Average(m => m.Precipitation).Value, 2) : null,
                    UVI = group.Average(m => m.UVI) != null ? Math.Round(group.Average(m => m.UVI).Value, 2) : null,
                    Light = group.Average(m => m.Light) != null ? Math.Round(group.Average(m => m.Light).Value, 2) : null,
                    Part03 = group.Average(m => m.Part03) != null ? Math.Round(group.Average(m => m.Part03).Value, 2) : null,
                    Part05 = group.Average(m => m.Part05) != null ? Math.Round(group.Average(m => m.Part05).Value, 2) : null,
                    Part10 = group.Average(m => m.Part10) != null ? Math.Round(group.Average(m => m.Part10).Value, 2) : null,
                    Part25 = group.Average(m => m.Part25) != null ? Math.Round(group.Average(m => m.Part25).Value, 2) : null,
                    Part50 = group.Average(m => m.Part50) != null ? Math.Round(group.Average(m => m.Part50).Value, 2) : null,
                    Part100 = group.Average(m => m.Part100) != null ? Math.Round(group.Average(m => m.Part100).Value, 2) : null,
                    PM10 = group.Average(m => m.PM10) != null ? Math.Round(group.Average(m => m.PM10).Value, 2) : null,
                    PM25 = group.Average(m => m.PM25) != null ? Math.Round(group.Average(m => m.PM25).Value, 2) : null,
                    PM100 = group.Average(m => m.PM100) != null ? Math.Round(group.Average(m => m.PM100).Value, 2) : null,
                    CO2 = group.Average(m => m.CO2) != null ? Math.Round(group.Average(m => m.CO2).Value, 2) : null
                };

                // Add the averaged measurement to the response list
                resp.Add(averagedMeasurement);
            }
        }
        catch (Exception ex)
        {
            // Optionally handle the exception (for logging or debugging)
            Console.WriteLine(ex.Message);
        }

        return resp;
    }
}
