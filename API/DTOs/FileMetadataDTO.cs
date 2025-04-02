using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Dtos;

[method: JsonConstructor]
[BsonIgnoreExtraElements]
public class FileMetadataDto(int? id, string? fileType, string? fileName, float? fileSize, int? duration, DateTime? date, string? checkSum)
{
    [BsonElement("id")]
    public int? Id { get; set; } = id;
    [BsonElement("file_type")]
    public string? FileType { get; set; } = fileType;
    [BsonElement("file_name")] 
    public string? FileName { get; set; } = fileName;
    [BsonElement("file_size")]
    public float? FileSize { get; set; } = fileSize;
    [BsonElement("duration")]
    public int? Duration { get; set; } = duration;
    [BsonElement("date")]
    public DateTime? Date { get; set; } = date;
    [BsonElement("checksum")]
    public string? CheckSum { get; set; } = checkSum;
}