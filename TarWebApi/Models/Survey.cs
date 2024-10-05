using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TarWebApi.Models.Enum;

namespace TarWebApi.Models;

[BsonIgnoreExtraElements]
public class Survey
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("surveyLanguage")]
    public SurveyLanguage? SurveyLanguage { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

}

public class SurveyLanguage
{
    [BsonElement("language")]
    public string Language { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("image")]
    public string? Image { get; set; }

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("questions")]
    public List<SurveyQuestion>? Questions { get; set; }
}

public class SurveyQuestion
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    [BsonElement("question")]
    public string Question { get; set; } = string.Empty;

    [BsonElement("type")]
    public int Type { get; set; }

    [BsonElement("answers")]
    public List<SurveyQuestionAnswer>? Answers { get; set; }

    [BsonElement("comment")]
    public string? Comment { get; set; }
}

public class SurveyQuestionAnswer
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    [BsonElement("answer")]
    public string Answer { get; set; } = string.Empty;
}