using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TarWebApi.Models;

[BsonIgnoreExtraElements]
public class Survey
{
    [BsonId]
    [BsonRepresentation(BsonType.String)] // Adjust to treat 'id' as a string
    public string Id { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("image")]
    public string? Image { get; set; }

    [BsonElement("category")]
    public string Category { get; set; }

    [BsonElement("questions")]
    public List<SurveyQuestion> Questions { get; set; }
}

public class SurveyQuestion
{
    [BsonElement("id")]
    [BsonRepresentation(BsonType.String)] // Ensure id is treated as a string
    public string Id { get; set; }

    [BsonElement("question")]
    public string Question { get; set; }

    [BsonElement("type")]
    public int Type { get; set; }

    [BsonElement("answers")]
    public List<SurveyAnswer> Answers { get; set; }

    [BsonElement("comment")]
    public string? Comment { get; set; }
}

public class SurveyAnswer
{
    [BsonElement("id")]
    [BsonRepresentation(BsonType.String)] // Ensure id is treated as a string
    public string Id { get; set; }

    [BsonElement("answer")]
    public string Answer { get; set; }

    [BsonElement("selected")]
    public bool Selected { get; set; }
}