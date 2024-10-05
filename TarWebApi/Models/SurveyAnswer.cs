using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TarWebApi.Models.Enum;

namespace TarWebApi.Models;

[BsonIgnoreExtraElements]
public class SurveyAnswer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("surveyId")]
    public string SurveyId { get; set; }

    [BsonElement("deviceId")]
    public string DeviceId { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("questions")]
    public List<SurveyAnswerQuestion> SurveyAnswerQuestions { get; set; }
}

public class SurveyAnswerQuestion
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    [BsonElement("answers")]
    public List<string> Answers { get; set; } = new List<string>();

    [BsonElement("comment")]
    public string? Comment { get; set; }
}