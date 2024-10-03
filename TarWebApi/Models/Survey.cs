using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TarWebApi.Models;

[BsonIgnoreExtraElements]
public class Survey
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

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
    public List<Question> Questions { get; set; }
}

public class Question
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("question")]
    public string QuestionText { get; set; }

    [BsonElement("type")]
    public int Type { get; set; }

    [BsonElement("answers")]
    public List<Answer> Answers { get; set; }

    [BsonElement("comment")]
    public string? Comment { get; set; }
}

public class Answer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("answer")]
    public string AnswerText { get; set; }

    [BsonElement("selected")]
    public bool Selected { get; set; }
}