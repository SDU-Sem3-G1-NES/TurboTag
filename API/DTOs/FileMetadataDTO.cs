using System.Text.Json.Serialization;

namespace API.Dtos;

[method: JsonConstructor]
public class FileMetadataDto(int id, string fileType, string fileName, float fileSize, int? duration, DateTime date, string checkSum)
{
    public int Id { get; } = id;
    public string FileType { get; } = fileType;
    public string FileName { get; } = fileName;
    public float FileSize { get; } = fileSize;
    public int? Duration { get; } = duration;
    public DateTime Date { get; } = date;
    public string CheckSum { get; } = checkSum;
}