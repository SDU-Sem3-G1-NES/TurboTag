using API.Dtos;

namespace API.Repositories
{
    public interface ILibraryRepository : IRepositoryBase
    {
        int AddLibrary(LibraryDto library);
        LibraryDto GetLibraryById(int libraryId);
        List<LibraryDto> GetAllLibraries();
        void UpdateLibrary(LibraryDto library);
        void DeleteLibrary(int libraryId);
    }
    public class LibraryRepository : ILibraryRepository
    {
        public int AddLibrary(LibraryDto library)
        {
            return 1;
        }
        public LibraryDto GetLibraryById(int libraryId)
        {
            return new LibraryDto(1, "Mock Library 1");
        }
        public List<LibraryDto> GetAllLibraries()
        {
            return new List<LibraryDto>
            {
                new LibraryDto(1, "Mock Library 1"),
                new LibraryDto(2, "Mock Library 2"),
            };
        }
        public void UpdateLibrary(LibraryDto library)
        {
            throw new NotImplementedException();
        }
        public void DeleteLibrary(int libraryId)
        {
            throw new NotImplementedException();
        }
    }
}