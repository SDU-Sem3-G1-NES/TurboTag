namespace API.DTOs;

public class UploadDetailsDTO(int uploadId, string uploadDescription, string uploadTitle, List<string> uploadTags)
{
    public int Id { get; set; } = uploadId;
    public string Description { get; set; } = uploadDescription;
    public string Title { get; set; } = uploadTitle;
    public List<string> Tags { get; set; } = uploadTags;
}