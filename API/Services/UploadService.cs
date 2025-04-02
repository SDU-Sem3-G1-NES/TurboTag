using API.Repositories;
using API.DTOs;

namespace API.Services;
public interface IUploadService : IServiceBase
{
    int StoreUpload();
}
public class UploadService(IUploadRepository uploadRepository) : IUploadService
{
    /// <summary>
    /// Method that stores an upload to the database and returns the BlobId of the upload.
    /// </summary>
    public int StoreUpload()
    {
        return uploadRepository.AddUpload(new UploadDto(1, 1, DateTime.Now, "image/png", 1));
    }
}