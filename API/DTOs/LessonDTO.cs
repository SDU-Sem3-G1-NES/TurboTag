using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.DTOs;

[BsonIgnoreExtraElements]
public class LessonDto
{
    public LessonDto()
    {
        UploadId = new List<int>();
        LessonDetails = new LessonDetailsDto();
        FileMetadata = new List<FileMetadataDto>();
    }
    
    [method: JsonConstructor]
    public LessonDto(List<int>? uploadId, LessonDetailsDto? lessonDetails, List<FileMetadataDto>? fileMetadata,
        int? ownerId)
    {
        UploadId = uploadId;
        LessonDetails = lessonDetails;
        FileMetadata = fileMetadata;
        OwnerId = ownerId;
    }
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? MongoId { get; set; }

    [BsonElement("upload_id")]
    public List<int>? UploadId { get; set; }
    [BsonElement("lesson_details")]
    public LessonDetailsDto? LessonDetails { get; set; }
    [BsonElement("file_metadata")]
    public List<FileMetadataDto>? FileMetadata { get; set; }
    [BsonElement("owner_id")]
    public int? OwnerId { get; set; }
    public void GenerateMongoId()
    {
        MongoId = ObjectId.GenerateNewId().ToString();
    }
} 