using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface IFileService : IServiceBase
{
    public Task<string?> UploadFile(IFormFile file);
    public Task<Stream?> GetFileById(string id);
    public Task DeleteFile(string id);
    public FileInfoDto? GetFileInfoByObjectId(string objectId);
    public void UpdateFileInfo(FileInfoDto fileInfo);
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
    
    public FileInfoDto? GetFileInfoByObjectId(string objectId)
    {
        return fileRepository.GetFileInfoByObjectId(objectId);
    }
    public void UpdateFileInfo(FileInfoDto fileInfo)
    {
        fileRepository.UpdateFileInfo(fileInfo);
    }
}