using API.DTOs;

namespace API.Repositories
{
    public interface IUserRepository : IRepositoryBase
    {
        int AddUser(UserDTO user);
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
    public class UserRepository : IUserRepository
    {
        public int AddUser(UserDTO user)
        {
            return 1;
        }
        public void AddUserCredentials(byte[] emailHash, byte[] passwordHash)
        {
            throw new NotImplementedException();
        }
        public bool UserEmailExists(string email)
        {
            return true;
        }
        public UserDTO GetUserById(int userId)
        {
            return new UserDTO(userId, 1, "mockuser@example.com", new List<string> { "Permission1", "Permission2" }, new List<SettingsDTO>());
        }
        public List<UserDTO> GetAllUsers()
        {
            return new List<UserDTO>
            {
                new UserDTO(1, 1, "mockuser1@example.com", new List<string> { "Permission1", "Permission2" }, new List<SettingsDTO>()),
                new UserDTO(2, 1, "mockuser2@example.com", new List<string> { "Permission1", "Permission2" }, new List<SettingsDTO>())
            };
        }
        public int GetUserId(string email)
        {
            return 1;
        }
        public byte[] GetHashedPassword(byte[] emailHash)
        {
            return new byte[] { 0x00, 0x01, 0x02, 0x03 };
        }
        public void UpdateUser(UserDTO user)
        {
            throw new NotImplementedException();
        }
        public void UpdateUserCredentials(byte[] emailHash, byte[] passwordHash)
        {
            throw new NotImplementedException();
        }
        public void DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }
        public void DeleteUserCredentials(byte[] passwordHash)
        {
            throw new NotImplementedException();
        }
    }
}