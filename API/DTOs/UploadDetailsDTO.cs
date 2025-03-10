namespace API.DTOs
{
    public class UploadDetailsDTO
    {
        public int uploadId { get; set; }
        public string uploadDescription { get; set; }
        public string uploadTitle { get; set; }
        public List<string> uploadTags { get; set; }
        public UploadDetailsDTO(int uploadId, string uploadDescription, string uploadTitle, List<string> uploadTags)
        {
            this.uploadId = uploadId;
            this.uploadDescription = uploadDescription;
            this.uploadTitle = uploadTitle;
            this.uploadTags = uploadTags;
        }
    }
}