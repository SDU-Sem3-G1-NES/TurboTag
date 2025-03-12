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
            new UploadDTO(1, 1, new UploadDetailsDTO(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDTO(1, "mp4", "Mock FileName 1", 2.5f, 1000, "2025-01-01", "Mock checkSum 1")),
            new UploadDTO(2, 1, new UploadDetailsDTO(2, "Mock Description 2", "Mock Title 2", new List<string> { "Mock Tag 2" }), new FileMetadataDTO(2, "mp4", "Mock FileName 2", 2.5f, 1000, "2025-01-01", "Mock checkSum 2")),
        };
    }
    /// <summary>
    /// Method that returns an UploadDTO object that belongs to a Library by Id.
    /// </summary>
    public UploadDTO GetLibraryUploadById()
    {
        return new UploadDTO(1, 1, new UploadDetailsDTO(1, "Mock Description 1", "Mock Title 1", new List<string> { "Mock Tag 1" }), new FileMetadataDTO(1, "mp4", "Mock FileName 1", 2.5f, 1000, "2025-01-01", "Mock checkSum 1"));
    }
}