using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace API.DTOs;

[BsonIgnoreExtraElements]
public class LessonDetailsDto
{
    public LessonDetailsDto()
    {
        Title = "";
        Description = "";
        Tags = new List<string>();
        ThumbnailId = "";
    }
    
    [method: JsonConstructor]
    public LessonDetailsDto(int? id, string? title, string? description, List<string>? tags, string? thumbnailId = "")
    {
        Id = id;
        Title = title;
        Description = description;
        Tags = tags;
    }
    [BsonElement("id")]
    public int? Id { get; set; }
    [BsonElement("title")]
    public string? Title { get; set; }
    [BsonElement("description")]
    public string? Description { get; set; }
    [BsonElement("tags")]
    public List<string>? Tags { get; set; }
    [BsonElement("thumbnail_id")]
    public string? ThumbnailId { get; set; }
}