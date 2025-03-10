using API.DTOs;

<<<<<<< HEAD
namespace API.Services;
public class UserCredentialsService : IUserCredentialsService
{
    /// <summary>
    /// Method that checks if the user exists in the database by input email.
    /// </summary>
    public bool CheckIfUserExistsByEmail()
    {
        return false;
    }
    /// <summary>
    /// Method that stores hashed password and email in the database.
    /// </summary>
    public void StoreNewUserCredentials()
    {
    }
    /// <summary>
    /// Method that validates user credentials by hashed input email and password.
    /// </summary>
    public bool ValidateUserCredentials()
    {
        return false;
    }
    /// <summary>
    /// Method that gets user data from the database and returns it as a UserDTO object.
    /// </summary>
    public UserDTO GetUserDataByEmail()
    {
        return new UserDTO(1, 1, "mock@mock.com", new List<string> { "mockPermission" }, new List<SettingsDTO> { new SettingsDTO(1, "mockSetting", "mockValue") });
    }
    /// <summary>
    /// Method that stores a user session in the database.
    /// </summary>
    public void StoreUserSession()
    {
    }
    /// <summary>
    /// Metho that gets user data from the database by session id and returns it as a UserDTO object.
    /// </summary>
    public UserDTO GetUserDataBySession()
    {
        return new UserDTO(1, 1, "mock@mock.com", new List<string> { "mockPermission" }, new List<SettingsDTO> { new SettingsDTO(1, "mockSetting", "mockValue") });
=======
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
>>>>>>> c1b482d (added services for frontend)
    }
}