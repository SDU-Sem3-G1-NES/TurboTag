using API.Dtos;
using API.Repositories;

namespace API.Services;

public interface IFileService : IServiceBase
{
    public Task<string?> UploadFile(IFormFile file);
    public Task<byte[]?> GetFile(string id);
    public Task DeleteFile(string id);
}
public class FileService(IFileRepository fileRepository) : IFileService
{
    public Task<string?> UploadFile(IFormFile file)
    {
        return fileRepository.UploadFile(file);
    }

    public Task<byte[]?> GetFile(string id)
    {
        return fileRepository.GetFile(id);
    }

    public Task DeleteFile(string id)
    {
        return fileRepository.DeleteFile(id);
    }
}