using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Dtos;

[BsonIgnoreExtraElements]
public class LessonDto(List<int>? uploadId, LessonDetailsDto? lessonDetails, List<FileMetadataDto>? fileMetadata, int? ownerId)
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? MongoId { get; set; }

    [BsonElement("upload_id")]
    public List<int>? UploadId { get; set; } = uploadId;
    [BsonElement("lesson_details")]
    public LessonDetailsDto? LessonDetails { get; set; } = lessonDetails;
    [BsonElement("file_metadata")]
    public List<FileMetadataDto>? FileMetadata { get; set; } = fileMetadata;
    [BsonElement("owner_id")]
    public int? OwnerId { get; set; } = ownerId;
    public void GenerateMongoId()
    {
        MongoId = ObjectId.GenerateNewId().ToString();
    }
} 