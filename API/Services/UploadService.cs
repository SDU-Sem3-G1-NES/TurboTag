using API.Repositories;
using API.DTOs;

namespace API.Services;
public interface IUploadService : IServiceBase
{
    string StoreUpload();
}
public class UploadService(IUploadRepository _uploadRepository) : IUploadService
{
    /// <summary>
    /// Method that stores an upload to the database and returns the BlobId of the upload.
    /// </summary>
    public string StoreUpload()
    {
        return _uploadRepository.AddUpload(new UploadDTO(1, 1, 1, new UploadDetailsDTO(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDTO(1, "mp4", "Mock FileName 1", 2.5f, 1000, new DateTime(2025,1,1), "Mock checkSum 1")));
    }
}