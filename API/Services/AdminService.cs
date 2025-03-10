using API.DTOs;

namespace API.Services
{
    public class AdminService
    {
        public List<UserDTO> GetAllUsers()
        {
            // Method that returns a list of all Users from the database.
            return new List<UserDTO>()
            {
                new UserDTO(1, 1, "mock@mock.com", new List<string> { "mockPermission" }, new List<SettingsDTO> { new SettingsDTO(1, "mockSetting", "mockValue") }),
                new UserDTO(2, 1, "mock2@mock.com", new List<string> { "mockPermission" }, new List<SettingsDTO> { new SettingsDTO(1, "mockSetting", "mockValue") })
            };
        }
        public List<LibraryDTO> GetAllLibraries()
        {
            // Method that returns a list of all Libraries from the database.
            return new List<LibraryDTO>()
            {
                new LibraryDTO(1, "mockLibrary"),
                new LibraryDTO(2, "mockLibrary2")
            };
        }
        public void CreateNewUser()
        {
            // Method that creates a new User in the database.
        }
        public void CreateNewUsers()
        {
            // Method that creates multiple new Users in the database.
        }
        public void UpdateUserById()
        {
            // Method that updates a User in the database by Id.
        }
        public void UpdateUsersByIds()
        {
            // Method that updates multiple Users in the database by Ids.
        }
        public void DeleteUserById()
        {
            // Method that deletes a User in the database by Id.
        }
        public void DeleteUsersByIds()
        {
            // Method that deletes multiple Users in the database by Ids.
        }



    }
}