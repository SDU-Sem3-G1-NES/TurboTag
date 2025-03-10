namespace API.DTOs
{
    public class UploadDetailsDTO
    {
        public required int uploadId { get; set; }
        public required string uploadDescription { get; set; }
        public required string uploadTitle { get; set; }
        public required List<string> uploadTags { get; set; }
    }
}