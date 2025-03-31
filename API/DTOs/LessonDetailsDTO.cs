using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Dtos;

[method: JsonConstructor]
[BsonIgnoreExtraElements]
public class LessonDetailsDto(int id, string title, string description, List<string> tags)
{
    [BsonElement("id")]
    public int Id { get; set; } = id;
    [BsonElement("title")]
    public string Title { get; set; } = title;
    [BsonElement("description")]
    public string Description { get; set; } = description;
    [BsonElement("tags")]
    public List<string> Tags { get; set; } = tags;
}