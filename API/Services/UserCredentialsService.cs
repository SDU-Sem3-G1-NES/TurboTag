using API.DTOs;

namespace API.Services;
public class UserCredentialsService : IService
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
    }
}