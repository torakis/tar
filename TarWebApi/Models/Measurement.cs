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

    // Timezone ID for UTC+2. Change as needed.
    private const string TimeZoneId = "Central European Standard Time";

    public DateTime Date { get; set; }
    
    [BsonElement("date")]
    public DateTime LocalDate
    {
        get
        {
            // Get the TimeZoneInfo for UTC+2
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            // Convert the UTC date to local time
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(Date, timeZoneInfo);
            // Handle edge case: If before 2 AM, consider it part of the previous day
            return localDateTime.Hour < 2 ? localDateTime.Date.AddDays(-1) : localDateTime.Date;
        }
    }

    [BsonElement("timestamp")]
    [JsonPropertyName("t")]
    public long? Timestamp { get; set; }

    [BsonElement("temperature")]
    public decimal? Temperature { get; set; }

    [BsonElement("humidity")]
    public decimal? Humidity { get; set; }

    [BsonElement("co2")]
    public decimal? CO2 { get; set; }

    [BsonElement("pm25")]
    public decimal? PM25 { get; set; }

    [BsonElement("pressure")]
    public decimal? Pressure { get; set; }

    [BsonElement("precipitation")]
    public decimal? Precipitation { get; set; }

    [BsonElement("windSpeed")]
    [JsonPropertyName("Wind_speed")]
    public decimal? WindSpeed { get; set; }

    [BsonElement("windDirection")]
    [JsonPropertyName("Wind_direction")]
    public decimal? WindDirection { get; set; }

    [BsonElement("gust")]
    public decimal? Gust { get; set; }

    [BsonElement("uvi")]
    public decimal? UVI { get; set; }

    [BsonElement("light")]
    public decimal? Light { get; set; }

    [BsonElement("part03")]
    public decimal? Part03 { get; set; }

    [BsonElement("part05")]
    public decimal? Part05 { get; set; }

    [BsonElement("part10")]
    public decimal? Part10 { get; set; }

    [BsonElement("part25")]
    public decimal? Part25 { get; set; }

    [BsonElement("part50")]
    public decimal? Part50 { get; set; }

    [BsonElement("part100")]
    public decimal? Part100 { get; set; }

    [BsonElement("pm10")]
    public decimal? PM10 { get; set; }

    [BsonElement("pm100")]
    public decimal? PM100 { get; set; }

    [BsonElement("lat")]
    public string? lat { get; set; }

    [BsonElement("lon")]
    public string? lon { get; set; }

    [BsonElement("alt")]
    public string? alt { get; set; }
}

public class MeasurementProjection
{
    public DateTime Date { get; set; }
    public decimal? Value { get; set; }
    
    // Timezone ID for UTC+2. Change as needed.
    private static readonly string TimeZoneId = "Central European Standard Time";

    public DateTime LocalDate
    {
        get
        {
            // Get the TimeZoneInfo for UTC+2
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

            // Convert the UTC date to local time
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(Date, timeZoneInfo);

            // Handle edge case: If before 2 AM, consider it part of the previous day
            if (localDateTime.Hour < 2)
            {
                return localDateTime.Date.AddDays(-1);
            }

            return localDateTime.Date;
        }
    }
}
