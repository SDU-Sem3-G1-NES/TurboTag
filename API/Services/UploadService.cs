using API.Repositories;
using API.Dtos;

namespace API.Services;
public interface IUploadService : IServiceBase
{
    string StoreUpload();
}
public class UploadService(IUploadRepository uploadRepository) : IUploadService
{
    /// <summary>
    /// Method that stores an upload to the database and returns the BlobId of the upload.
    /// </summary>
    public string StoreUpload()
    {
        return uploadRepository.AddUpload(new UploadDto(1, 1, 1, new UploadDetailsDto(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDto(1, "mp4", "Mock FileName 1", 2.5f, 1000, new DateTime(2025,1,1), "Mock checkSum 1")));
    }
}