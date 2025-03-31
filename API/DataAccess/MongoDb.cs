using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace API.DataAccess;

public interface IMongoDb
{
    List<T> Find<T>(string collectionName, string query);
    void Insert<T>(string collectionName, T document);
    void Replace<T>(string collectionName, string query, T document);
    void Delete(string collectionName, string query);
}
public class MongoDb : IMongoDb
{
    private readonly IMongoClient _client;
    public MongoDb(string connectionString)
    {
        _client = new MongoClient(connectionString);
    }
    public List<T> Find<T>(string collectionName, string query)
    {
        IMongoDatabase database = _client.GetDatabase(GetDatabaseName());
        try
        {
            var filter = BsonDocument.Parse(query);
            var collection = database.GetCollection<T>(collectionName);
            List<T> result = collection.Find(filter).ToList();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<T>();
        }
    }
    public void Insert<T>(string collectionName, T document)
    {
        IMongoDatabase database = _client.GetDatabase(GetDatabaseName());
        try 
        {
            var collection = database.GetCollection<T>(collectionName);
            collection.InsertOne(document);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public void Replace<T>(string collectionName, string query, T document)
    {
        IMongoDatabase database = _client.GetDatabase(GetDatabaseName());
        try 
        {
            var filter = BsonDocument.Parse(query);
            var collection = database.GetCollection<T>(collectionName);
            //document.Remove("_id");
            collection.ReplaceOne(filter, document);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public void Delete(string collectionName, string query)
    {
        IMongoDatabase database = _client.GetDatabase(GetDatabaseName());
        try 
        {
            var filter = BsonDocument.Parse(query);
            var collection = database.GetCollection<BsonDocument>(collectionName);
            collection.DeleteMany(filter);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    private string GetDatabaseName()
    {
        return "blank";
    }
}