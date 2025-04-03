using API.Repositories;

namespace API.Services;

public interface IFileService : IServiceBase
{
    public Task<string?> UploadFile(IFormFile file);
    public Task<Stream?> GetFile(string id);
    public Task DeleteFile(string id);
}
public class FileService(IFileRepository fileRepository) : IFileService
{
    public Task<string?> UploadFile(IFormFile file)
    {
        return fileRepository.UploadFile(file);
    }

    public Task<Stream?> GetFile(string id)
    {
        return fileRepository.GetFile(id);
    }

    public Task DeleteFile(string id)
    {
        return fileRepository.DeleteFile(id);
    }
}