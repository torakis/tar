using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TarWebApi.Models.Enum;

namespace TarWebApi.Models;

[BsonIgnoreExtraElements]
public class SurveyAnswer
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("surveyid")]
    public string SurveyId { get; set; }

    [BsonElement("deviceid")]
    public string DeviceId { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("questions")]
    public List<SurveyQuestion> Questions { get; set; }
}