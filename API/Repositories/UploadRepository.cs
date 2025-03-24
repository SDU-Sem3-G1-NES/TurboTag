using API.DTOs;

namespace API.Repositories
{
    public interface IUploadRepository : IRepositoryBase
    {
        string AddUpload(UploadDTO upload);
        UploadDTO GetUploadById(int uploadId);
        List<UploadDTO> GetUploadsByOwnerId(int ownerId);
        List<UploadDTO> GetUploadsByLibraryId(int libraryId);
        List<UploadDTO> GetAllUploads();
        void UpdateUpload(UploadDTO upload);
        void DeleteUploadById(int uploadId);
        void DeleteUploadsByOwnerId(int ownerId);
        void DeleteUploadsByLibraryId(int libraryId);
    }
    public class UploadRepository : IUploadRepository
    {
        public string AddUpload(UploadDTO upload)
        {
            return "MockBlobId";
        }
        public UploadDTO GetUploadById(int uploadId)
        {
            return new UploadDTO(1, 1, 1, new UploadDetailsDTO(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDTO(1, "mp4", "Mock FileName 1", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 1"));
        }
        public List<UploadDTO> GetUploadsByOwnerId(int ownerId)
        {
            return new List<UploadDTO>
            {
                new UploadDTO(1, 1, 1, new UploadDetailsDTO(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDTO(1, "mp4", "Mock FileName 1", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 1")),
                new UploadDTO(2, 1, 1, new UploadDetailsDTO(2, "Mock Description 2", "Mock Title 2", new List<string> { "Mock Tag 2" }), new FileMetadataDTO(2, "mp4", "Mock FileName 2", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 2")),
            };
        }
        public List<UploadDTO> GetUploadsByLibraryId(int libraryId)
        {
            return new List<UploadDTO>
            {
                new UploadDTO(1, 1, libraryId, new UploadDetailsDTO(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDTO(1, "mp4", "Mock FileName 1", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 1")),
                new UploadDTO(2, 1, libraryId, new UploadDetailsDTO(2, "Mock Description 2", "Mock Title 2", new List<string> { "Mock Tag 2" }), new FileMetadataDTO(2, "mp4", "Mock FileName 2", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 2")),
            };
        }
        public List<UploadDTO> GetAllUploads()
        {
            return new List<UploadDTO>
            {
                new UploadDTO(1, 1, 1, new UploadDetailsDTO(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDTO(1, "mp4", "Mock FileName 1", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 1")),
                new UploadDTO(2, 1, 1, new UploadDetailsDTO(2, "Mock Description 2", "Mock Title 2", new List<string> { "Mock Tag 2" }), new FileMetadataDTO(2, "mp4", "Mock FileName 2", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 2")),
            };
        }
        public void UpdateUpload(UploadDTO upload)
        {
            throw new NotImplementedException();
        }
        public void DeleteUploadById(int uploadId)
        {
            throw new NotImplementedException();
        }
        public void DeleteUploadsByOwnerId(int ownerId)
        {
            throw new NotImplementedException();
        }
        public void DeleteUploadsByLibraryId(int libraryId)
        {
            throw new NotImplementedException();
        }
    }
}