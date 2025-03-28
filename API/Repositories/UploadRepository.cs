using API.DTOs;

namespace API.Repositories
{
    public interface IUploadRepository : IRepositoryBase
    {
        string AddUpload(UploadDto upload);
        UploadDto GetUploadById(int uploadId);
        List<UploadDto> GetUploadsByOwnerId(int ownerId);
        List<UploadDto> GetUploadsByLibraryId(int libraryId);
        List<UploadDto> GetAllUploads();
        void UpdateUpload(UploadDto upload);
        void DeleteUploadById(int uploadId);
        void DeleteUploadsByOwnerId(int ownerId);
        void DeleteUploadsByLibraryId(int libraryId);
    }
    public class UploadRepository : IUploadRepository
    {
        public string AddUpload(UploadDto upload)
        {
            return "MockBlobId";
        }
        public UploadDto GetUploadById(int uploadId)
        {
            return new UploadDto(1, 1, 1, new UploadDetailsDto(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDto(1, "mp4", "Mock FileName 1", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 1"));
        }
        public List<UploadDto> GetUploadsByOwnerId(int ownerId)
        {
            return new List<UploadDto>
            {
                new UploadDto(1, 1, 1, new UploadDetailsDto(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDto(1, "mp4", "Mock FileName 1", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 1")),
                new UploadDto(2, 1, 1, new UploadDetailsDto(2, "Mock Description 2", "Mock Title 2", new List<string> { "Mock Tag 2" }), new FileMetadataDto(2, "mp4", "Mock FileName 2", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 2")),
            };
        }
        public List<UploadDto> GetUploadsByLibraryId(int libraryId)
        {
            return new List<UploadDto>
            {
                new UploadDto(1, 1, libraryId, new UploadDetailsDto(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDto(1, "mp4", "Mock FileName 1", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 1")),
                new UploadDto(2, 1, libraryId, new UploadDetailsDto(2, "Mock Description 2", "Mock Title 2", new List<string> { "Mock Tag 2" }), new FileMetadataDto(2, "mp4", "Mock FileName 2", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 2")),
            };
        }
        public List<UploadDto> GetAllUploads()
        {
            return new List<UploadDto>
            {
                new UploadDto(1, 1, 1, new UploadDetailsDto(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDto(1, "mp4", "Mock FileName 1", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 1")),
                new UploadDto(2, 1, 1, new UploadDetailsDto(2, "Mock Description 2", "Mock Title 2", new List<string> { "Mock Tag 2" }), new FileMetadataDto(2, "mp4", "Mock FileName 2", 2.5f, 1000, new DateTime(2025, 1, 1), "Mock checkSum 2")),
            };
        }
        public void UpdateUpload(UploadDto upload)
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