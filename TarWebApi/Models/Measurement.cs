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
    public string? StationId { get; set; }

    [BsonElement("date")]
    public DateTime? Date { get; set; }

    [BsonElement("timestamp")]
    [JsonPropertyName("t")]
    public long? Timestamp { get; set; }

    [BsonElement("temperature")]
    public decimal? Temperature { get; set; }

    [BsonElement("humidity")]
    public decimal? Humidity { get; set; }

    [BsonElement("pressure")]
    public int? Pressure { get; set; }

    [BsonElement("windSpeed")]
    [JsonPropertyName("Wind_speed")]
    public int? WindSpeed { get; set; }

    [BsonElement("windDirection")]
    [JsonPropertyName("Wind_direction")]
    public int? WindDirection { get; set; }

    [BsonElement("Gust")]
    public decimal? Gust { get; set; }

    [BsonElement("precipitation")]
    public int? Precipitation { get; set; }

    [BsonElement("uvi")]
    public decimal? UVI { get; set; }

    [BsonElement("part03")]
    public int? Part03 { get; set; }

    [BsonElement("part05")]
    public int? Part05 { get; set; }
}

