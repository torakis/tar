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

    public async Task<CreateMeasurementResponse> CreateMeasurementAsync(CreateMeasurementRequest request)
    {
        var resp = new CreateMeasurementResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            await _measurementsCollection.InsertOneAsync(request.Measurement);
            resp.Measurement = request.Measurement;
        }
        catch (Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.ToString();
        }
        return resp;
    }

    public async Task<GetMeasurementsByIdResponse> GetMeasurementsByIdAsync(GetMeasurementsByIdRequest request)
    {
        var resp = new GetMeasurementsByIdResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            var measurements = await _measurementsCollection.Find(s => s.StationId == request.Id).ToListAsync();
            if (measurements is null)
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Measurements for station with Id = {request.Id} not found";
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

    //Gets the measurements for specific station for the requested period
    //A valid day is from 00:00 - 23:59
    public async Task<GetMeasurementsByPeriodResponse> GetMeasurementsByPeriodAsync(GetMeasurementsByPeriodRequest request)
    {
        var resp = new GetMeasurementsByPeriodResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            var searchDate = CalculateSearchDateForPeriod(request.Period);
            var measurements = await _measurementsCollection.Find(s => s.StationId == request.Id && s.Date >= searchDate).ToListAsync();
            if (measurements is null)
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Measurements for station with Id = {request.Id} not found for the requested period.";
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
            var searchDate = CalculateSearchDateForPeriod(request.Period);
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

    private static DateTime CalculateSearchDateForPeriod(Period period)
    {
        var searchDate = DateTime.Now;
        switch (period)
        {
            case Period.Day:
                searchDate = searchDate.AddDays(-1);
                break;
            case Period.Week:
                searchDate = searchDate.AddDays(-7);
                break;
            case Period.Month:
                searchDate = searchDate.AddMonths(-1);
                break;
        }
        return searchDate;
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
