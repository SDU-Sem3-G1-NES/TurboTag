using API.DTOs;
using API.Repositories;

namespace API.Services;
public interface ILibraryService : IServiceBase
{
    IEnumerable<LibraryDto> GetLibrariesByUser(UserDto user, LibraryFilter filter);
    IEnumerable<LibraryDto> GetAllLibraries(LibraryFilter? filter);
    LibraryDto GetLibraryById(int libraryId);
    void CreateNewLibrary(LibraryDto library);
    void UpdateLibrary(LibraryDto library);
    void DeleteLibraryById(int libraryId);
}
public class LibraryService(ILibraryRepository libraryRepository) : ILibraryService
{
    public IEnumerable<LibraryDto> GetLibrariesByUser(UserDto user, LibraryFilter filter)
    {
        filter.LibraryIds = user.AccessibleLibraryIds;
        return libraryRepository.GetAllLibraries(filter);
    }
    
    public IEnumerable<LibraryDto> GetAllLibraries(LibraryFilter? filter)
    {
        return libraryRepository.GetAllLibraries(filter);
    }

    public LibraryDto GetLibraryById(int libraryId)
    {
        return libraryRepository.GetLibraryById(libraryId);
    }
    
    public void CreateNewLibrary(LibraryDto library)
    {
        libraryRepository.AddLibrary(library);
    }
    
    public void UpdateLibrary(LibraryDto library)
    {
        libraryRepository.UpdateLibrary(library);
    }
    
    public void DeleteLibraryById(int libraryId)
    {
        libraryRepository.DeleteLibraryById(libraryId);
    }
}