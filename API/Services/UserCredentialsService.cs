using API.DTOs;

namespace API.Services
{
    public class UserCredentialsService
    {
        // Methods for register page.
        public bool CheckIfUserExistsByEmail()
        {
            // Check if user exists in the database by input email.
            return false;
        }
        public void StoreNewUserCredentials()
        {
            // Store hashed password and email in the database.
        }

        // Methods for login page.
        public bool ValidateUserCredentials()
        {
            // Validate user credentials by hashed input email and password.
            return false;
        }
        public UserDTO GetUserDataByEmail()
        {
            // Get user data from the database and return it as a UserDTO object.
            return new UserDTO(1, 1, "mock@mock.com", new List<string> { "mockPermission" }, new List<SettingsDTO> { new SettingsDTO(1, "mockSetting", "mockValue") });
        }
        public void StoreUserSession()
        {
            // Store user session in the database.
        }
        public UserDTO GetUserDataBySession()
        {
            // Get user data from the database by session and return it as a UserDTO object.
            return new UserDTO(1, 1, "mock@mock.com", new List<string> { "mockPermission" }, new List<SettingsDTO> { new SettingsDTO(1, "mockSetting", "mockValue") });
        }
    }
}