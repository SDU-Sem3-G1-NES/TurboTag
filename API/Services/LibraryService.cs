using API.DTOs;

namespace API.Services;
public class LibraryService : ILibraryService
{
    /// <summary>
    /// Method that returns a list of LibraryDTO objects that belong to a User by Id.
    /// </summary>
    public List<LibraryDTO> GetUserLibrariesById()
    {
        return new List<LibraryDTO>
        {
            new LibraryDTO(1, "Mock Library 1"),
            new LibraryDTO(2, "Mock Library 2"),
        };
    }
    /// <summary>
    /// Method that returns a LibraryDTO object that belongs to a User by Id.
    /// </summary>
    public LibraryDTO GetUserLibraryById()
    {
        return new LibraryDTO(1, "Mock Library 1");
    }
    /// <summary>
    /// Method that returns a list of UploadDTO objects that belong to a Library by Id.
    /// </summary>
    public List<UploadDTO> GetLibraryUploadsById()
    {
        return new List<UploadDTO>
            {
                new UploadDTO(1, 1, 1, new UploadDetailsDTO(1, "MockFile1.txt", "/mock/path/MockFile1.txt", new List<string>()), new FileMetadataDTO(1, "MockFile1.txt", "/mock/path/MockFile1.txt", 0.0f, null, "MockType", "MockDescription")),
                new UploadDTO(2, 1, 1, new UploadDetailsDTO(2, "MockFile2.txt", "/mock/path/MockFile2.txt", new List<string>()), new FileMetadataDTO(2, "MockFile2.txt", "/mock/path/MockFile2.txt", 0.0f, null, "MockType", "MockDescription"))
            };
    }
    /// <summary>
    /// Method that returns an UploadDTO object that belongs to a Library by Id.
    /// </summary>
    public UploadDTO GetLibraryUploadById()
    {
        return new UploadDTO(1, 1, 1, new UploadDetailsDTO(1, "MockFile.txt", "/mock/path/MockFile.txt", new List<string>()), new FileMetadataDTO(1, "MockFile.txt", "/mock/path/MockFile.txt", 0.0f, null, "MockType", "MockDescription"));
    }
}