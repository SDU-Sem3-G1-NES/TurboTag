namespace API.DataAccess;

public interface IDocumentDbAccess
{
    List<T> Find<T>(string collectionName, string query);
    void Insert<T>(string collectionName, T document);
    void Replace<T>(string collectionName, string query, T document);
    void Delete(string collectionName, string query);
}