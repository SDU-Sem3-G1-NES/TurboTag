using API.DTOs;

namespace API.Repositories.Interfaces
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
}