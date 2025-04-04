namespace API.DataAccess;

public interface IMongoDataAccess
{
    List<T> Find<T>(string collectionName, string query);
    void Insert<T>(string collectionName, T document);
    void Replace<T>(string collectionName, string query, T document);
    void Delete(string collectionName, string query);
    Task<string?> UploadFile(string bucketName, IFormFile file);
    Task<Stream?> GetFileById(string bucketName, string id);
    Task DeleteFile(string bucketName, string id);
}