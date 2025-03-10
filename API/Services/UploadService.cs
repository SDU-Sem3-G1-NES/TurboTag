<<<<<<< HEAD
namespace API.Services;
public class UploadService : IUploadService
{
    /// <summary>
    /// Method that stores an upload to the database and returns the BlobId of the upload.
    /// </summary>
    public string StoreUpload()
    {
        return "MockBlobId";
=======
namespace API.Services
{
    public class UploadService
    {
        public void StoreUpload()
        {
            // Method that mocks storing an upload to the database.
        }
>>>>>>> c1b482d (added services for frontend)
    }
}