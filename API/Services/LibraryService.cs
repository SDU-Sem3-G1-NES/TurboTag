using API.DTOs;
using API.Repositories;

namespace API.Services;
public interface ILibraryService : IServiceBase
{
    List<LibraryDTO> GetUserLibrariesById();
    LibraryDTO GetUserLibraryById();
    List<UploadDTO> GetLibraryUploadsById();
    UploadDTO GetLibraryUploadById();
}
public class LibraryService(ILibraryRepository _libraryRepository, IUploadRepository _uploadRepository) : ILibraryService
{
    /// <summary>
    /// Method that returns a list of LibraryDTO objects that belong to a User by Id.
    /// </summary>
    public List<LibraryDTO> GetUserLibrariesById()
    {
        // bad method in the repository? should return a list of libraries by user id. returns all existing libraries instead.
        return _libraryRepository.GetAllLibraries();
    }
    /// <summary>
    /// Method that returns a LibraryDTO object that belongs to a User by Id.
    /// </summary>
    public LibraryDTO GetUserLibraryById()
    {
        return _libraryRepository.GetLibraryById(1);
    }
    /// <summary>
    /// Method that returns a list of UploadDTO objects that belong to a Library by Id.
    /// </summary>
    public List<UploadDTO> GetLibraryUploadsById()
    {
        return _uploadRepository.GetUploadsByLibraryId(1);
    }
    /// <summary>
    /// Method that returns an UploadDTO object that belongs to a Library by Id.
    /// </summary>
    public UploadDTO GetLibraryUploadById()
    {
        return _uploadRepository.GetUploadById(1);
    }
}