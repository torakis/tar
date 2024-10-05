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

    public async Task<GetMeasurementsByIdResponse> GetMeasurementsByIdAsync(GetMeasurementsByIdRequest request)
    {
        var resp = new GetMeasurementsByIdResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            var measurements = await _measurementsCollection.Find(s => s.StationId == request.StationId).ToListAsync();
            if (measurements is null)
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Measurements for station with Id = {request.StationId} not found";
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

    //Gets the measurements for specific station for the requested period
    //A valid day is from 00:00 - 23:59
    //calcuate the average depending on period
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

    //for one measurement type
    public async Task<GetMeasurementsByPeriodResponse> GetMeasurementByPeriodAsync(GetMeasurementByPeriodRequest request)
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

    public async Task<GetPeriodStatisticsResponse> GetPeriodStatisticsAsync(GetPeriodStatisticsRequest request)
    {
        var resp = new GetPeriodStatisticsResponse() { Statistics = new List<Statistics>(), IsSuccessful = true, ErrorText = "" };
        try
        {
            var searchDate = DateTime.Now;
            var measurements = await _measurementsCollection.Find(s => s.StationId == request.Id && s.Date >= searchDate).ToListAsync();
            if (measurements is null)
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Measurements for station with Id = {request.Id} not found for the requested period.";
            }
            else
            {
                var statistics = new List<Statistics>();
                var dayMeasurements = measurements.Where(m => m.Date == searchDate).ToList();

                for (DateTime date = searchDate; date <= DateTime.Now; date += TimeSpan.FromDays(1))
                {
                    dayMeasurements = measurements.Where(m => m.Date == date).ToList();
                    statistics.Add(CalculateStatisticsForDay(dayMeasurements));
                }

                resp.Statistics.AddRange(statistics);
            }
        }
        catch (Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.ToString();
        }
        return resp;
    }

    //ypologismos statistics
    //      returns enum day, min, max, average (1 value)
    //    //returns enum week, min, max, average (7 values)
    //    //returns enum month, min, max, average (30 values)
    private static Statistics CalculateStatisticsForDay(List<Measurement> measurements)
    {
        var statistics = new Statistics()
        {
            Day = measurements.FirstOrDefault()?.Date,
            Temperature = new StatisticsDetails()
            {
                Min = measurements.Min(m => m.Temperature),
                Max = measurements.Max(m => m.Temperature),
                Avg = measurements.Average(m => m.Temperature)
            },
            Humidity = new StatisticsDetails()
            {
                Min = measurements.Min(m => m.Humidity),
                Max = measurements.Max(m => m.Humidity),
                Avg = measurements.Average(m => m.Humidity)
            },
            Co2 = new StatisticsDetails()
            {
                Min = measurements.Min(m => m.CO2),
                Max = measurements.Max(m => m.CO2),
                Avg = (decimal?)measurements.Average(m => m.CO2)
            },
            Pm25 = new StatisticsDetails()
            {
                Min = measurements.Min(m => m.PM25),
                Max = measurements.Max(m => m.PM25),
                Avg = measurements.Average(m => m.PM25)
            },
            Pressure = new StatisticsDetails()
            {
                Min = measurements.Min(m => m.Pressure),
                Max = measurements.Max(m => m.Pressure),
                Avg = (decimal?)measurements.Average(m => m.Pressure)
            },
            Precipitation = new StatisticsDetails()
            {
                Min = measurements.Min(m => m.Precipitation),
                Max = measurements.Max(m => m.Precipitation),
                Avg = (decimal?)measurements.Average(m => m.Precipitation)
            },
            WindSpeed = new StatisticsDetails()
            {
                Min = measurements.Min(m => m.WindSpeed),
                Max = measurements.Max(m => m.WindSpeed),
                Avg = (decimal?)measurements.Average(m => m.WindSpeed)
            },
            WindDirection = new StatisticsDetails()
            {
                Min = measurements.Min(m => m.WindDirection),
                Max = measurements.Max(m => m.WindDirection),
                Avg = (decimal?)measurements.Average(m => m.WindDirection)
            }
        };

        return statistics;
    }
}
