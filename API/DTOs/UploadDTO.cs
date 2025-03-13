namespace API.DTOs;
public class UploadDTO(int uploadId, int ownerId, UploadDetailsDTO uploadDetails, FileMetadataDTO fileMetadata)
{
    public int Id { get; } = uploadId;
    public int OwnerId { get; } = ownerId;
    public UploadDetailsDTO Details { get; } = uploadDetails;
    public FileMetadataDTO FileMetadata { get; } = fileMetadata;
}
