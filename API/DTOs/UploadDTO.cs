namespace API.DTOs;
public class UploadDTO(int uploadId, int ownerId, int libraryId, UploadDetailsDTO uploadDetails, FileMetadataDTO fileMetadata)
{
    public int Id { get; } = uploadId;
    public int OwnerId { get; } = ownerId;
    public int LibraryId { get; } = libraryId;
    public UploadDetailsDTO Details { get; } = uploadDetails;
    public FileMetadataDTO FileMetadata { get; } = fileMetadata;
}
