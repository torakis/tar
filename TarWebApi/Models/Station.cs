using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TarWebApi.Models;

[BsonIgnoreExtraElements]
public class Station
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } = string.Empty;

    [BsonElement("stationId")]
    public string? StationId { get; set; }

    [BsonElement("stationType")]
    public int? StationType { get; set; }

    [BsonElement("stationName")]
    public string? StationName { get; set; }

    [BsonElement("latitude")]
    public decimal? Latitude { get; set; }

    [BsonElement("longitude")]
    public decimal? Longitude { get; set; }
}

