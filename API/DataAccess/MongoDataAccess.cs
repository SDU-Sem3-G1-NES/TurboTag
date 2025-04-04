using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace API.DataAccess;

public class MongoDataAccess(string connectionString) : IMongoDataAccess
{
    private readonly IMongoClient _client = new MongoClient(connectionString);

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
    
    public async Task<string?> UploadFile(string bucketName, IFormFile file)
    {
        IMongoDatabase database = _client.GetDatabase(GetDatabaseName());
        try
        {
            var bucketOptions = new GridFSBucketOptions
            {
                BucketName = bucketName
            };
            GridFSBucket bucket = new(database, bucketOptions);
            
            await using var stream = await bucket.OpenUploadStreamAsync(file.FileName);
            var id = stream.Id;
            await file.CopyToAsync(stream);
            await stream.CloseAsync();
            return id.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<Stream?> GetFileById(string bucketName, string id)
    {
        try
        {
            var bucket = GetGridFsBucketByName(bucketName);
            var fileInfo = FindFile(bucket, id);
            if (fileInfo == null)
            {
                Console.WriteLine("File not found");
                return null;
            }
            return await bucket.OpenDownloadStreamAsync(fileInfo.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task DeleteFile(string bucketName, string id)
    {
        try
        {
            var bucket = GetGridFsBucketByName(bucketName);
            var fileInfo = FindFile(bucket, id);
            if (fileInfo == null)
            {
                Console.WriteLine("File not found");
                return;
            }
            await bucket.DeleteAsync(fileInfo.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private GridFSFileInfo? FindFile(GridFSBucket bucket, string id)
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", new ObjectId(id));
        var fileInfo = bucket.Find(filter).FirstOrDefault();
        return fileInfo;
    }
    
    private GridFSBucket GetGridFsBucketByName(string bucketName)
    {
        IMongoDatabase database = _client.GetDatabase(GetDatabaseName());
        var bucketOptions = new GridFSBucketOptions
        {
            BucketName = bucketName
        };
        return new GridFSBucket(database, bucketOptions);
    }
    
    private string GetDatabaseName()
    {
        return "blank";
    }
}