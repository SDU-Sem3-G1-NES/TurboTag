namespace API.DTOs;

public class UploadDetailsDTO(int uploadId, string uploadDescription, string uploadTitle, List<string> uploadTags)
{
    public int Id { get; } = uploadId;
    public string Description { get; } = uploadDescription;
    public string Title { get; } = uploadTitle;
    public List<string> Tags { get; } = uploadTags;
}