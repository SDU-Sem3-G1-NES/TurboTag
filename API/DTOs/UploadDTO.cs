namespace API.DTOs;

public class UploadDTO(
    int uploadId,
    int ownerId,
    int libraryId,
    UploadDetailsDTO uploadDetails,
    FileMetadataDTO fileMetadata)
{
    public int Id { get; set; } = uploadId;
    public int OwnerId { get; set; } = ownerId;
    public int LibraryId { get; set; } = libraryId;
    public UploadDetailsDTO Details { get; set; } = uploadDetails;
    public FileMetadataDTO FileMetadata { get; set; } = fileMetadata;
}