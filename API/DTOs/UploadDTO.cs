namespace API.DTOs
{
    public class UploadDTO
    {
        public int uploadId { get; set; }
        public int ownerId { get; set; }
        public UploadDetailsDTO uploadDetails { get; set; }
        public FileMetadataDTO fileMetadata { get; set; }

        public UploadDTO(int uploadId, int ownerId, UploadDetailsDTO uploadDetails, FileMetadataDTO fileMetadata)
        {
            this.uploadId = uploadId;
            this.ownerId = ownerId;
            this.uploadDetails = uploadDetails;
            this.fileMetadata = fileMetadata;
        }
    }
}