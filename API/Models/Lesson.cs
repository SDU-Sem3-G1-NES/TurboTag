using MongoDB.Bson.Serialization.Attributes;

namespace API.Models;

[BsonIgnoreExtraElements]
public class Lesson(string id, LessonDetails lessonDetails, FileMetadata fileMetadata, string ownerId)
{
    [BsonId]
    public string? Id { get; set; } = id;
    [BsonElement("lesson_details")]
    public LessonDetails LessonDetails { get; set; } = lessonDetails;
    [BsonElement("file_metadata")]
    public FileMetadata FileMetadata { get; set; } = fileMetadata;
    [BsonElement("owner_id")]
    public string OwnerId { get; set; } = ownerId;
} 

[BsonIgnoreExtraElements]
public class LessonDetails(string id, string title, string description, List<string> tags)
{
    [BsonId]
    public string? Id { get; set; } = id;
    [BsonElement("title")]
    public string Title { get; set; } = title;
    [BsonElement("description")]
    public string Description { get; set; } = description;
    [BsonElement("tags")]
    public List<string> Tags { get; set; } = tags;
}

[BsonIgnoreExtraElements]
public class FileMetadata(string id, string fileName, string fileType, float size, DateTime uploadDate, string checkSum, int? duration, string? resolution, string? pageCount)
{
    [BsonId]
    public string? Id { get; set; } = id;
    [BsonElement("fileName")] 
    public string FileNam { get; set; } = fileName;
    [BsonElement("fileType")]
    public string FileType { get; set; } = fileType;
    [BsonElement("size")]
    public float Size { get; set; } = size;
    [BsonElement("uploadDate")]
    public DateTime UploadDate { get; set; } = uploadDate;
    [BsonElement("checkSum")]
    public string CheckSum { get; set; } = checkSum;
    [BsonElement("duration")]
    public int? Duration { get; set; } = duration;
    [BsonElement("resolution")]
    public string? Resolution { get; set; } = resolution;
    [BsonElement("pageCount")]
    public string? PageCount { get; set; } = pageCount;
}