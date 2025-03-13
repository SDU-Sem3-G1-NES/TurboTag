using API.DTOs;

namespace API.Repositories
{
    public interface ILibraryRepository : IRepositoryBase
    {
        void AddLibrary(LibraryDTO library);
        LibraryDTO GetLibraryById(int libraryId);
        List<LibraryDTO> GetAllLibraries();
        void UpdateLibrary(LibraryDTO library);
        void DeleteLibrary(int libraryId);
    }
    public class LibraryRepository : ILibraryRepository
    {
        public void AddLibrary(LibraryDTO library)
        {
            throw new System.NotImplementedException();
        }
        public LibraryDTO GetLibraryById(int libraryId)
        {
            throw new System.NotImplementedException();
        }
        public List<LibraryDTO> GetAllLibraries()
        {
            throw new System.NotImplementedException();
        }
        public void UpdateLibrary(LibraryDTO library)
        {
            throw new System.NotImplementedException();
        }
        public void DeleteLibrary(int libraryId)
        {
            throw new System.NotImplementedException();
        }
    }
}