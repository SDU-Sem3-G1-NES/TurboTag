using API.DataAccess;
using API.DTOs;
using MongoDB.Bson;

namespace API.Repositories;

public interface IFileRepository : IRepositoryBase
{
    Task<String?> UploadFile(IFormFile file);
    Task<Stream?> GetFileById(string id);
    Task DeleteFile(string id);
    FileInfoDto? GetFileInfoByObjectId(string objectId);
    void UpdateFileInfo(FileInfoDto fileInfo);
}
public class FileRepository(IMongoDataAccess database) : IFileRepository 
{
    
    public async Task<string?> UploadFile(IFormFile file)
    {
        return await database.UploadFile("fs", file);
    }

    public async Task<Stream?> GetFileById(string id)
    {
        return await database.GetFileById("fs", id);
    }

    public async Task DeleteFile(string id)
    {
        await database.DeleteFile("fs", id);
    }
    
    public FileInfoDto? GetFileInfoByObjectId(string objectId)
    {
        if (!ObjectId.TryParse(objectId, out _))
        {
            Console.WriteLine("Invalid ObjectId format provided while fetching a lesson.");
            return null;
        }
        return database.Find<FileInfoDto>(
                "fs.files",
                $"{{\"_id\": ObjectId(\"{objectId}\")}}")
            .FirstOrDefault();
    }
    
    public void UpdateFileInfo(FileInfoDto fileInfo)
    {
        database.Replace("fs.files", $"{{\"_id\": ObjectId(\"{fileInfo.MongoId}\")}}", fileInfo);
    }
}