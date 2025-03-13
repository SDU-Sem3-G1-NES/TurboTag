using API.DTOs;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository : IRepositoryBase
    {
        void AddUser(UserDTO user);
        void AddUserCredentials(byte[] emailHash, byte[] passwordHash);
        bool UserEmailExists(string email);
        UserDTO GetUserById(int userId);
        List<UserDTO> GetAllUsers();
        int GetUserId(string email);
        byte[] GetHashedPassword(byte[] emailHash);
        void UpdateUser(UserDTO user);
        void UpdateUserCredentials(byte[] emailHash, byte[] passwordHash);
        void DeleteUser(int userId);
        void DeleteUserCredentials(byte[] passwordHash);
    }
}