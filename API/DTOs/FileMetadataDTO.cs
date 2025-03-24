namespace API.DTOs;

public class FileMetadataDTO(
    int uploadId,
    string fileType,
    string fileName,
    float fileSize,
    int? duration,
    string uploadDate,
    string checkSum)
{
    public int Id { get; set; } = uploadId;
    public string FileType { get; set; } = fileType;
    public string FileName { get; set; } = fileName;
    public float FileSize { get; set; } = fileSize;
    public int? Duration { get; set; } = duration;
    public string Date { get; set; } = uploadDate;
    public string CheckSum { get; set; } = checkSum;
}