using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace API.DataAccess;

public class MongoDataAccess(string connectionString) : IMongoDataAccess
{
    private readonly IMongoClient _client = new MongoClient(connectionString);

    public List<T> Find<T>(string collectionName, string query, int? skip = null, int? limit = null)
    {
        var database = _client.GetDatabase(GetDatabaseName());
        try
        {
            var filter = BsonDocument.Parse(query);
            var collection = database.GetCollection<T>(collectionName);
            var findFluent = collection.Find(filter);

            if (skip.HasValue)
                findFluent = findFluent.Skip(skip.Value);
            if (limit.HasValue)
                findFluent = findFluent.Limit(limit.Value);
            
            return findFluent.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<T>();
        }
    }

    public void Insert<T>(string collectionName, T document)
    {
        var database = _client.GetDatabase(GetDatabaseName());
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
        var database = _client.GetDatabase(GetDatabaseName());
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
        var database = _client.GetDatabase(GetDatabaseName());
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
        var database = _client.GetDatabase(GetDatabaseName());
        try
        {
            var bucketOptions = new GridFSBucketOptions
            {
                BucketName = bucketName
            };
            GridFSBucket bucket = new(database, bucketOptions);

            await using var uploadStream = await bucket.OpenUploadStreamAsync(file.FileName);
            using var fileStream = file.OpenReadStream(); // This avoids buffering the full file in memory
            await fileStream.CopyToAsync(uploadStream); // Stream directly from request -> GridFS
            await uploadStream.CloseAsync();

            return uploadStream.Id.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<string?> UploadChunkedFile(string bucketName, Stream stream, string fileName)
    {
        var database = _client.GetDatabase(GetDatabaseName());
        try
        {
            var bucketOptions = new GridFSBucketOptions
            {
                BucketName = bucketName
            };
            GridFSBucket bucket = new(database, bucketOptions);

            // Ensure unique filename
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, fileName);
            var existingFile = bucket.Find(filter).FirstOrDefault();

            string fileNameToUse = fileName;
            if (existingFile != null)
            {
                fileNameToUse = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            }

            await using var uploadStream = await bucket.OpenUploadStreamAsync(fileNameToUse);
            await stream.CopyToAsync(uploadStream);
            await uploadStream.CloseAsync();

            return uploadStream.Id.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error uploading file: {e.Message}");
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
        var database = _client.GetDatabase(GetDatabaseName());
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