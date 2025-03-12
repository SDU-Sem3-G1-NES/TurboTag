using API.DTOs;

namespace API.Services;
public interface IAdminService : IServiceBase
    {
        List<UserDTO> GetAllUsers();
        UserDTO GetUserById();
        List<LibraryDTO> GetAllLibraries();
        LibraryDTO GetLibraryById();
        void CreateNewUser();
        void CreateNewUsers();
        void UpdateUserById();
        void UpdateUsersByIds();
        void DeleteUserById();
        void DeleteUsersByIds();
    }