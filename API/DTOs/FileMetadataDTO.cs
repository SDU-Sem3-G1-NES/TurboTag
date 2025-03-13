namespace API.DTOs;
public class FileMetadataDTO(int uploadId, string fileType, string fileName, float fileSize, int? duration, string uploadDate, string checkSum)
{
    public int Id { get; } = uploadId;
    public string FileType { get; } = fileType;
    public string FileName { get; } = fileName;
    public float FileSize { get; } = fileSize;
    public int? Duration { get; } = duration;
    public string Date { get; } = uploadDate;
    public string CheckSum { get; } = checkSum;
}