using System.Text.Json.Serialization;

namespace API.Dtos;

[method: JsonConstructor]
public class UploadDto(
    int id,
    int ownerId,
    int libraryId,
    UploadDetailsDto details,
    FileMetadataDto fileMetadata)
{
    public int Id { get; set; } = id;
    public int OwnerId { get; set; } = ownerId;
    public int LibraryId { get; set; } = libraryId;
    public UploadDetailsDto Details { get; set; } = details;
    public FileMetadataDto FileMetadata { get; set; } = fileMetadata;
}