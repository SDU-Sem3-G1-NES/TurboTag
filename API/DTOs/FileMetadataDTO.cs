using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace API.DTOs;

[BsonIgnoreExtraElements]
public class FileMetadataDto
{
    public FileMetadataDto()
    {
        FileType = "";
        FileName = "";
        CheckSum = "";
    }

    [method: JsonConstructor]
    public FileMetadataDto(string? id, string? fileType, string? fileName, float? fileSize, int? duration, DateTime? date,
        string? checkSum)
    {
        Id = id;
        FileType = fileType;
        FileName = fileName;
        FileSize = fileSize;
        Duration = duration;
        Date = date;
        CheckSum = checkSum;
    }

    [BsonElement("id")]
    public string? Id { get; set; }
    [BsonElement("file_type")]
    public string? FileType { get; set; }
    [BsonElement("filename")] 
    public string? FileName { get; set; }
    [BsonElement("file_size")]
    public float? FileSize { get; set; }
    [BsonElement("duration")]
    public int? Duration { get; set; }
    [BsonElement("date")]
    public DateTime? Date { get; set; }
    [BsonElement("checksum")]
    public string? CheckSum { get; set; }
}