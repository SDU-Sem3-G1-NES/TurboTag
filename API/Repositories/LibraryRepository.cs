using API.DTOs;

namespace API.Repositories
{
    public interface ILibraryRepository : IRepositoryBase
    {
        int AddLibrary(LibraryDTO library);
        LibraryDTO GetLibraryById(int libraryId);
        List<LibraryDTO> GetAllLibraries();
        void UpdateLibrary(LibraryDTO library);
        void DeleteLibrary(int libraryId);
    }
    public class LibraryRepository : ILibraryRepository
    {
        public int AddLibrary(LibraryDTO library)
        {
            return 1;
        }
        public LibraryDTO GetLibraryById(int libraryId)
        {
            return new LibraryDTO(1, "Mock Library 1");
        }
        public List<LibraryDTO> GetAllLibraries()
        {
            return new List<LibraryDTO>
            {
                new LibraryDTO(1, "Mock Library 1"),
                new LibraryDTO(2, "Mock Library 2"),
            };
        }
        public void UpdateLibrary(LibraryDTO library)
        {
            throw new NotImplementedException();
        }
        public void DeleteLibrary(int libraryId)
        {
            throw new NotImplementedException();
        }
    }
}