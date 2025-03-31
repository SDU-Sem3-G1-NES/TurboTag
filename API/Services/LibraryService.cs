using API.Dtos;
using API.Repositories;

namespace API.Services;
public interface ILibraryService : IServiceBase
{
    List<LibraryDto> GetUserLibrariesById();
    LibraryDto GetUserLibraryById();
    List<UploadDto> GetLibraryUploadsById();
    UploadDto GetLibraryUploadById();
}
public class LibraryService(ILibraryRepository libraryRepository, IUploadRepository uploadRepository) : ILibraryService
{
    /// <summary>
    /// Method that returns a list of LibraryDto objects that belong to a User by Id.
    /// </summary>
    public List<LibraryDto> GetUserLibrariesById()
    {
        return libraryRepository.GetAllLibraries();
    }
    /// <summary>
    /// Method that returns a LibraryDto object that belongs to a User by Id.
    /// </summary>
    public LibraryDto GetUserLibraryById()
    {
        return libraryRepository.GetLibraryById(1);
    }
    /// <summary>
    /// Method that returns a list of UploadDto objects that belong to a Library by Id.
    /// </summary>
    public List<UploadDto> GetLibraryUploadsById()
    {
        return uploadRepository.GetUploadsByLibraryId(1);
    }
    /// <summary>
    /// Method that returns an UploadDto object that belongs to a Library by Id.
    /// </summary>
    public UploadDto GetLibraryUploadById()
    {
        return uploadRepository.GetUploadById(1);
    }
}