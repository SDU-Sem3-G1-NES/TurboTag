using API.DTOs;

namespace API.Repositories.Interfaces
{
    public interface ILibraryRepository : IRepositoryBase
    {
        void AddLibrary(LibraryDTO library);
        LibraryDTO GetLibraryById(int libraryId);
        List<LibraryDTO> GetAllLibraries();
        void UpdateLibrary(LibraryDTO library);
        void DeleteLibrary(int libraryId);
    }
}