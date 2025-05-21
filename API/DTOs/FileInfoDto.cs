using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.DTOs;

public class FileInfoDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? MongoId { get; set; }
    
    [BsonElement("length")]
    public int Length { get; set; }
    
    [BsonElement("chunkSize")]
    public int ChunkSize { get; set; }
    
    [BsonElement("uploadDate")]
    public DateTime UploadDate { get; set; }
    
    [BsonElement("filename")]
    public string? Filename { get; set; }
    
    [BsonElement("transcription")]
    public string? Transcription { get; set; }
    
    public FileInfoDto()
    {
    }
    
    public FileInfoDto(string mongoId, int length, int chunkSize, DateTime uploadDate, string filename, string? transcription)
    {
        MongoId = mongoId;
        Length = length;
        ChunkSize = chunkSize;
        UploadDate = uploadDate;
        Filename = filename;
        Transcription = transcription;
    }
    
    public void GenerateMongoId()
    {
        MongoId = ObjectId.GenerateNewId().ToString();
    }
}