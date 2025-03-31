using API.Dtos;

namespace API.Repositories
{
    public interface IUserRepository : IRepositoryBase
    {
        int AddUser(UserDto user);
        void AddUserCredentials(byte[] emailHash, byte[] passwordHash);
        bool UserEmailExists(string email);
        UserDto GetUserById(int userId);
        List<UserDto> GetAllUsers();
        int GetUserId(string email);
        byte[] GetHashedPassword(byte[] emailHash);
        void UpdateUser(UserDto user);
        void UpdateUserCredentials(byte[] emailHash, byte[] passwordHash);
        void DeleteUser(int userId);
        void DeleteUserCredentials(byte[] passwordHash);
        void StoreUserSession();
        UserDto GetUserBySession();
        UserDto GetUserByEmail(string email);
    }
    public class UserRepository : IUserRepository
    {
        public int AddUser(UserDto user)
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
        public UserDto GetUserById(int userId)
        {
            return new UserDto(userId, 1, "mockuser@example.com", new List<string> { "Permission1", "Permission2" }, new List<SettingsDto>());
        }
        public List<UserDto> GetAllUsers()
        {
            return new List<UserDto>
            {
                new UserDto(1, 1, "mockuser1@example.com", new List<string> { "Permission1", "Permission2" }, new List<SettingsDto>()),
                new UserDto(2, 1, "mockuser2@example.com", new List<string> { "Permission1", "Permission2" }, new List<SettingsDto>())
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
        public void UpdateUser(UserDto user)
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
        public void StoreUserSession()
        {
            throw new NotImplementedException();
        }
        public UserDto GetUserBySession()
        {
            throw new NotImplementedException();
        }
        public UserDto GetUserByEmail(string email)
        {
            return new UserDto(1, 1, email, new List<string> { "Permission1", "Permission2" }, new List<SettingsDto>());
        }
    }
}