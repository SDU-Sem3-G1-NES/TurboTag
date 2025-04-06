using API.Repositories;
using API.DTOs;

namespace API.Services;
public interface IUploadService : IServiceBase
{
    IEnumerable<UploadDto> GetAllUploads(UploadFilter? filter);
    IEnumerable<UploadDto> GetUserUploads(UserDto user, UploadFilter filter);
    IEnumerable<UploadDto> GetLibraryUploads(int libraryId, UploadFilter filter);
    UploadDto GetUploadById(int uploadId);
    void CreateNewUpload(UploadDto upload);
    void UpdateUpload(UploadDto upload);
    void DeleteUploadById(int uploadId);
    
}
public class UploadService(IUploadRepository uploadRepository) : IUploadService
{
    public IEnumerable<UploadDto> GetAllUploads(UploadFilter? filter)
    {
        return uploadRepository.GetAllUploads(filter);
    }
    
    public IEnumerable<UploadDto> GetUserUploads(UserDto user, UploadFilter filter)
    {
        filter.OwnerIds = [user.Id];
        return uploadRepository.GetAllUploads(filter);
    }
    
    public IEnumerable<UploadDto> GetLibraryUploads(int libraryId, UploadFilter filter)
    {
        filter.LibraryIds = [libraryId];
        return uploadRepository.GetAllUploads(filter);
    }

    public UploadDto GetUploadById(int uploadId)
    {
        return uploadRepository.GetUploadById(uploadId);
    }

    public void CreateNewUpload(UploadDto upload)
    {
        uploadRepository.AddUpload(upload);
    }
    
    public void UpdateUpload(UploadDto upload)
    {
        uploadRepository.UpdateUpload(upload);
    }
    
    public void DeleteUploadById(int uploadId)
    {
        uploadRepository.DeleteUploadById(uploadId);
    }
}