using API.Repositories;
using API.DTOs;

namespace API.Services;
public interface IUploadService : IServiceBase
{
    void StoreUpload(UploadDto uploadDto);
}
public class UploadService(IUploadRepository _uploadRepository) : IUploadService
{
    /// <summary>
    /// Method that stores an upload to the database and returns the BlobId of the upload.
    /// </summary>
    public void StoreUpload(UploadDto uploadDto)
    {
        _uploadRepository.AddUpload(uploadDto);
    }
}