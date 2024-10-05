using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using Amazon.Runtime.Internal;
using Microsoft.VisualBasic;
using MongoDB.Bson;
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
            // Define the filter for the query
            var filter = Builders<Measurement>.Filter.Where(s => s.StationId == request.StationId
                && s.Date >= request.DateFrom
                && s.Date <= request.DateTo);

            var dateDiff = (request.DateTo - request.DateFrom)?.TotalDays ?? 0;

            // Determine the grouping interval based on the date difference
            string interval;
            if (dateDiff < 1)
            {
                interval = "hour";  // Group by hour if the date range is less than 1 day
            }
            else if (dateDiff >= 1 && dateDiff < 7)
            {
                interval = "4hours";  // Group by 4 hours if the date range is between 1 and 7 days
            }
            else
            {
                interval = "day";  // Group by day if the date range is greater than 7 days
            }

            // Map the selected MeasurementType to the corresponding field in the database
            string measurementField = request.MeasurementType switch
            {
                MeasurementType.Temperature => "Temperature",
                MeasurementType.Humidity => "Humidity",
                MeasurementType.Pressure => "Pressure",
                MeasurementType.WindSpeed => "WindSpeed",
                MeasurementType.WindDirection => "WindDirection",
                MeasurementType.Gust => "Gust",
                MeasurementType.Precipitation => "Precipitation",
                MeasurementType.UVI => "UVI",
                MeasurementType.Light => "Light",
                MeasurementType.CO2 => "CO2",
                _ => throw new ArgumentOutOfRangeException(nameof(request.MeasurementType), "Unsupported MeasurementType")
            };

            // Build the aggregation pipeline
            var pipeline = new List<BsonDocument>
        {
            // Match documents based on the filter
            new BsonDocument("$match", filter.ToBsonDocument()),

            // Group by the time interval and calculate the average value of the selected measurement type
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", new BsonDocument
                    {
                        { "$dateTrunc", new BsonDocument
                            {
                                { "date", "$date" },
                                { "unit", interval == "4hours" ? "hour" : interval }, // Use "hour" for both hour and 4-hour intervals
                                { "binSize", interval == "4hours" ? 4 : 1 }  // If 4 hours, set binSize to 4
                            }
                        }
                    }
                },
                { "averageMeasurement", new BsonDocument { { "$avg", $"${measurementField}" } } },  // Average value of the selected field
                { "measurements", new BsonDocument { { "$push", "$$ROOT" } } }  // Optionally push all matching measurements into an array
            })
        };

            // Execute the aggregation query
            var measurements = await _measurementsCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            if (measurements == null || !measurements.Any())
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Measurements for station {request.StationId} not found for the requested period.";
            }
            else
            {
                // Map the aggregated data into your response model
                resp.Measurements = measurements.Select(m => new MeasurementProjection
                {
                    Date = m["_id"].ToUniversalTime(),  // The grouped date
                    Value = m["averageMeasurement"].AsDecimal  // The average value for the selected measurement type
                }).ToList();
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
