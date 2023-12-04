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
    [JsonPropertyName("stationId")]
    public string? StationId { get; set; }

    [BsonElement("stationType")]
    [JsonPropertyName("stationType")]
    public int? StationType { get; set; }

    [BsonElement("stationName")]
    [JsonPropertyName("stationName")]
    public string? StationName { get; set; }

    [BsonElement("latitude")]
    [JsonPropertyName("latitude")]
    public decimal? Latitude { get; set; }

    [BsonElement("longitude")]
    [JsonPropertyName("longitude")]
    public decimal? Longitude { get; set; }

    [BsonElement("temperature")]
    [JsonPropertyName("temperature")]
    public bool? Temperature { get; set; }

    [BsonElement("humidity")]
    [JsonPropertyName("humidity")]
    public bool? Humidity { get; set; }

    [BsonElement("co2")]
    [JsonPropertyName("co2")]
    public bool? Co2 { get; set; }

    [BsonElement("pm25")]
    [JsonPropertyName("pm25")]
    public bool? Pm25 { get; set; }

    [BsonElement("pressure")]
    [JsonPropertyName("pressure")]
    public bool? Pressure { get; set; }

    [BsonElement("precipitation")]
    [JsonPropertyName("precipitation")]
    public bool? Precipitation { get; set; }

    [BsonElement("windSpeed")]
    [JsonPropertyName("windSpeed")]
    public bool? WindSpeed { get; set; }

    [BsonElement("windDirection")]
    [JsonPropertyName("windDirection")]
    public bool? WindDirection { get; set; }
}

