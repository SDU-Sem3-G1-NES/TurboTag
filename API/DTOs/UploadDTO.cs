using System.Text.Json.Serialization;

namespace API.DTOs
{
    public class UploadDTO
    {
        public int? Id { get; set; }
        public int? OwnerId { get; set; }
        public int? LibraryId { get; set; }
        public UploadDetailsDTO Details { get; set; }
        public FileMetadataDTO FileMetadata { get; set; }

        [JsonConstructor]
        public UploadDTO(int? id, int? ownerId, int? libraryId, UploadDetailsDTO details, FileMetadataDTO fileMetadata)
        {
            Id = id;
            OwnerId = ownerId;
            LibraryId = libraryId;
            Details = details;
            FileMetadata = fileMetadata;
        }
    }
}