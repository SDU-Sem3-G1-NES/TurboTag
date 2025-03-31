using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Dtos;

[method: JsonConstructor]
[BsonIgnoreExtraElements]
public class FileMetadataDto(int id, string fileType, string fileName, float fileSize, int duration, DateTime date, string checkSum)
{
    [BsonElement("id")]
    public int Id { get; } = id;
    [BsonElement("file_type")]
    public string FileType { get; } = fileType;
    [BsonElement("file_name")] 
    public string FileName { get; } = fileName;
    [BsonElement("file_size")]
    public float FileSize { get; } = fileSize;
    [BsonElement("duration")]
    public int Duration { get; } = duration;
    [BsonElement("date")]
    public DateTime Date { get; } = date;
    [BsonElement("checksum")]
    public string CheckSum { get; } = checkSum;
}