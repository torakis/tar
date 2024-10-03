using MongoDB.Driver;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;

namespace TarWebApi.Services;

public class StationService : IStationService
{
    private readonly IMongoCollection<Station> _stationsCollection;
    public StationService(IStationStoreDatabaseSettings settings, IMongoClient mongoClient)
    {
        var db = mongoClient.GetDatabase(settings.DatabaseName);
        _stationsCollection = db.GetCollection<Station>(settings.StationsCollectionName);
    }

    public async Task<GetAllStationsResponse> GetAllStationsAsync(GetAllStationsRequest request)
    {
        var resp = new GetAllStationsResponse() { IsSuccessful = true, ErrorText = "" };
        var stations = await _stationsCollection.Find(_ => true).ToListAsync();
        if (stations is null)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = $"No stations found";
        }
        else
            resp.Stations = stations;
        return resp;
    }

    public async Task<GetStationByIdResponse> GetStationByIdAsync(GetStationByIdRequest request)
    {
        var resp = new GetStationByIdResponse() { IsSuccessful = true, ErrorText = "" };
        var station = await _stationsCollection.Find(s => s.StationId == request.Id).FirstOrDefaultAsync();
        if (station is null)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = $"Station with Id = {request.Id} not found";
        }
        resp.Station = station;
        return resp;
    }

    public async Task<CreateStationsResponse> CreateStationsAsync(CreateStationsRequest request)
    {
        var resp = new CreateStationsResponse() { IsSuccessful = true, ErrorText = "" };
        try
        {
            foreach(var station in request.Stations)
            {
                await _stationsCollection.InsertOneAsync(station);
            }            
        }
        catch (Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.Message?.ToString();
        }
        return resp;
    }
}
