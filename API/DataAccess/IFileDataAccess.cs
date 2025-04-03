namespace API.DataAccess;

public interface IFileDataAccess
{
    Task<string?> UploadFile(string bucketName, IFormFile file);
    Task<Byte[]?> GetFileById(string bucketName, string id);
    Task DeleteFile(string bucketName, string id);
}