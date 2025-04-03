using API.Repositories;

namespace API.Services;

public interface IFileService : IServiceBase
{
    public Task<string?> UploadFile(IFormFile file);
    public Task<Stream?> GetFileById(string id);
    public Task DeleteFile(string id);
}
public class FileService(IFileRepository fileRepository) : IFileService
{
    public Task<string?> UploadFile(IFormFile file)
    {
        return fileRepository.UploadFile(file);
    }

    public Task<Stream?> GetFileById(string id)
    {
        return fileRepository.GetFileById(id);
    }

    public Task DeleteFile(string id)
    {
        return fileRepository.DeleteFile(id);
    }
}