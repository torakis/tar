using MongoDB.Driver;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;
using static System.Collections.Specialized.BitVector32;

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

    public async Task<CreateStationResponse> CreateStationAsync(CreateStationRequest request)
    {
        var resp = new CreateStationResponse() { IsSuccessful = true, ErrorText = $"Station with Id = {request.Station.Id} was created" };
        try
        {
            await _stationsCollection.InsertOneAsync(request.Station);
            resp.Station = request.Station;
        }
        catch (Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.ToString();
        }
        return resp;
    }

    public async Task<UpdateStationResponse> UpdateStationAsync(UpdateStationRequest request)
    {
        var resp = new UpdateStationResponse() { IsSuccessful = true, ErrorText = $"Station with Id = {request.Station.Id} was updated" };
        try
        {
            var getStationByIdReq = new GetStationByIdRequest() { Id = request.Station.Id };
            var existingStation = await GetStationByIdAsync(getStationByIdReq);
            if (existingStation is null)
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Station with Id = {request.Station.Id} not found";
                return resp;
            }
            else
            {
                await _stationsCollection.ReplaceOneAsync(s => s.StationId == request.Station.Id, request.Station);
                resp.Station = request.Station;
            }
        }
        catch (Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.ToString();
        }
        return resp;
    }

    public async Task<DeleteStationResponse> DeleteStationAsync(DeleteStationRequest request)
    {
        var resp = new DeleteStationResponse() { IsSuccessful = true, ErrorText = $"Station with Id = {request.Id} was deleted" };
        try
        {
            var getStationByIdReq = new GetStationByIdRequest() { Id = request.Id };
            var existingStation = await GetStationByIdAsync(getStationByIdReq);
            if (existingStation is null)
            {
                resp.IsSuccessful = false;
                resp.ErrorText = $"Station with Id = {request.Id} not found";
                return resp;
            }
            else
            {
                await _stationsCollection.DeleteOneAsync(s => s.StationId == request.Id);
            }
        }
        catch (Exception ex)
        {
            resp.IsSuccessful = false;
            resp.ErrorText = ex.ToString();
        }
        return resp;
    }
}
