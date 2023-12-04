using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TarWebApi.Models;

[BsonIgnoreExtraElements]
public class Measurement
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } = string.Empty;

    [BsonElement("stationId")]
    [JsonPropertyName("stationId")]
    public string? StationId { get; set; }

    [BsonElement("date")]
    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }

    [BsonElement("timestamp")]
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    [BsonElement("latitude")]
    [JsonPropertyName("latitude")]
    public decimal? Latitude { get; set; }

    [BsonElement("longitude")]
    [JsonPropertyName("longitude")]
    public decimal? Longitude { get; set; }

    [BsonElement("temperature")]
    [JsonPropertyName("temperature")]
    public decimal? Temperature { get; set; }

    [BsonElement("humidity")]
    [JsonPropertyName("humidity")]
    public decimal? Humidity { get; set; }

    [BsonElement("co2")]
    [JsonPropertyName("co2")]
    public int? Co2 { get; set; }

    [BsonElement("pm25")]
    [JsonPropertyName("pm25")]
    public decimal? Pm25 { get; set; }

    [BsonElement("pressure")]
    [JsonPropertyName("pressure")]
    public int? Pressure { get; set; }

    [BsonElement("precipitation")]
    [JsonPropertyName("precipitation")]
    public int? Precipitation { get; set; }

    [BsonElement("windSpeed")]
    [JsonPropertyName("windSpeed")]
    public int? WindSpeed { get; set; }

    [BsonElement("windDirection")]
    [JsonPropertyName("windDirection")]
    public int? WindDirection { get; set; }
}

