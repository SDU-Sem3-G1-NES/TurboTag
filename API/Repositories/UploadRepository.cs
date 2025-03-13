using API.DTOs;

namespace API.Repositories
{
    public interface IUploadRepository : IRepositoryBase
    {
        void AddUpload(UploadDTO upload);
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
        public void AddUpload(UploadDTO upload)
        {
            throw new NotImplementedException();
        }
        public UploadDTO GetUploadById(int uploadId)
        {
            return new UploadDTO(uploadId, 1, 1, new UploadDetailsDTO(uploadId, "MockFile.txt", "/mock/path/MockFile.txt", new List<string>()), new FileMetadataDTO(uploadId, "MockFile.txt", "/mock/path/MockFile.txt", 0.0f, null, "MockType", "MockDescription"));
        }
        public List<UploadDTO> GetUploadsByOwnerId(int ownerId)
        {
            return new List<UploadDTO>
            {
                new UploadDTO(1, ownerId, 1, new UploadDetailsDTO(1, "MockFile1.txt", "/mock/path/MockFile1.txt", new List<string>()), new FileMetadataDTO(1, "MockFile1.txt", "/mock/path/MockFile1.txt", 0.0f, null, "MockType", "MockDescription")),
                new UploadDTO(2, ownerId, 1, new UploadDetailsDTO(2, "MockFile2.txt", "/mock/path/MockFile2.txt", new List<string>()), new FileMetadataDTO(2, "MockFile2.txt", "/mock/path/MockFile2.txt", 0.0f, null, "MockType", "MockDescription"))
            };
        }
        public List<UploadDTO> GetUploadsByLibraryId(int libraryId)
        {
            return new List<UploadDTO>
            {
                new UploadDTO(1, 1, libraryId, new UploadDetailsDTO(1, "MockFile1.txt", "/mock/path/MockFile1.txt", new List<string>()), new FileMetadataDTO(1, "MockFile1.txt", "/mock/path/MockFile1.txt", 0.0f, null, "MockType", "MockDescription")),
                new UploadDTO(2, 1, libraryId, new UploadDetailsDTO(2, "MockFile2.txt", "/mock/path/MockFile2.txt", new List<string>()), new FileMetadataDTO(2, "MockFile2.txt", "/mock/path/MockFile2.txt", 0.0f, null, "MockType", "MockDescription"))
            };
        }
        public List<UploadDTO> GetAllUploads()
        {
            return new List<UploadDTO>
            {
                new UploadDTO(1, 1, 1, new UploadDetailsDTO(1, "MockFile1.txt", "/mock/path/MockFile1.txt", new List<string>()), new FileMetadataDTO(1, "MockFile1.txt", "/mock/path/MockFile1.txt", 0.0f, null, "MockType", "MockDescription")),
                new UploadDTO(2, 1, 1, new UploadDetailsDTO(2, "MockFile2.txt", "/mock/path/MockFile2.txt", new List<string>()), new FileMetadataDTO(2, "MockFile2.txt", "/mock/path/MockFile2.txt", 0.0f, null, "MockType", "MockDescription"))
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