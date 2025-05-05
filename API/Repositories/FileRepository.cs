using API.DataAccess;

namespace API.Repositories;

public interface IFileRepository : IRepositoryBase
{
    Task<string?> UploadChunkedFile(Stream stream, string fileName);
    Task<String?> UploadFile(IFormFile file);
    Task<Stream?> GetFileById(string id);
    Task DeleteFile(string id);
}
public class FileRepository(IMongoDataAccess database) : IFileRepository 
{
    
    private const string BucketName = "fs";

    public Task<string?> UploadChunkedFile(Stream stream, string fileName)
    {
        
        return database.UploadChunkedFile(BucketName, stream, fileName);
    }
    
    public async Task<string?> UploadFile(IFormFile file)
    {
        return await database.UploadFile(BucketName, file);
    }

    public async Task<Stream?> GetFileById(string id)
    {
        return await database.GetFileById(BucketName, id);
    }

    public async Task DeleteFile(string id)
    {
        await database.DeleteFile(BucketName, id);
    }
}