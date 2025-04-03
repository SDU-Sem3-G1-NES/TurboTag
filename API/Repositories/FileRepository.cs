using API.Dtos;
using API.DataAccess;

namespace API.Repositories;

public interface IFileRepository : IRepositoryBase
{
    Task<String?> UploadFile(IFormFile file);
    Task<Byte[]?> GetFile(string id);
    Task DeleteFile(string id);
}
public class FileRepository(IFileDataAccess database) : IFileRepository 
{
    
    public async Task<string?> UploadFile(IFormFile file)
    { ;
        return await database.UploadFile("fs", file);
    }

    public async Task<Byte[]?> GetFile(string id)
    {
        return await database.GetFileById("fs", id);
    }

    public async Task DeleteFile(string id)
    {
        await database.DeleteFile("fs", id);
    }
}