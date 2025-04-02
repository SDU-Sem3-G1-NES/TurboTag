using System.Text.Json.Serialization;

namespace API.DTOs;

public class FileMetadataDto
{
    public FileMetadataDto()
    {
        FileType = "";
        FileName = "";
        CheckSum = "";
    }

    [method: JsonConstructor]
    public FileMetadataDto(int id, string fileType, string fileName, float fileSize, int? duration, DateTime date,
        string checkSum)
    {
        Id = id;
        FileType = fileType;
        FileName = fileName;
        FileSize = fileSize;
        Duration = duration;
        Date = date;
        CheckSum = checkSum;
    }

    public int Id { get; }
    public string FileType { get; }
    public string FileName { get; }
    public float FileSize { get; }
    public int? Duration { get; }
    public DateTime Date { get; }
    public string CheckSum { get; }
}