namespace API.DTOs
{
    public class UploadDTO
    {
        public required int uploadId { get; set; }
        public required int ownerId { get; set; }
        public required UploadDetailsDTO uploadDetails { get; set; }
        public required FileMetadataDTO fileMetadata { get; set; }

        public UploadDTO(int uploadId, int ownerId, UploadDetailsDTO uploadDetails, FileMetadataDTO fileMetadata)
        {
            this.uploadId = uploadId;
            this.ownerId = ownerId;
            this.uploadDetails = uploadDetails;
            this.fileMetadata = fileMetadata;
        }
    }
}