namespace API.Services;
public class UploadService : IUploadService
{
    /// <summary>
    /// Method that stores an upload to the database and returns the BlobId of the upload.
    /// </summary>
    public string StoreUpload()
    {
        return "MockBlobId";
    }
}