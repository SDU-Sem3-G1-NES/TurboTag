using API.Dtos;
using API.Repositories;

namespace API.Services;
public interface IUserCredentialsService : IServiceBase
{
    bool CheckIfUserExistsByEmail();
    void StoreNewUserCredentials();
    bool ValidateUserCredentials();
    UserDto GetUserDataByEmail();
    void StoreUserSession();
    UserDto GetUserDataBySession();
}
public class UserCredentialsService(IUserRepository _userRepository) : IUserCredentialsService
{
    /// <summary>
    /// Method that checks if the user exists in the database by input email.
    /// </summary>
    public bool CheckIfUserExistsByEmail()
    {
        return _userRepository.UserEmailExists("mock@mock.com");
    }
    /// <summary>
    /// Method that stores hashed password and email in the database.
    /// </summary>
    public void StoreNewUserCredentials()
    {
        _userRepository.AddUserCredentials(new byte[] { 0x00, 0x01, 0x02, 0x03 }, new byte[] { 0x00, 0x01, 0x02, 0x03 });
    }
    /// <summary>
    /// Method that validates user credentials by hashed input email and password.
    /// </summary>
    public bool ValidateUserCredentials()
    {
        return true;
    }
    /// <summary>
    /// Method that gets user data from the database and returns it as a UserDto object.
    /// </summary>
    public UserDto GetUserDataByEmail()
    {
        return _userRepository.GetUserByEmail("mock@mock.com");
    }
    /// <summary>
    /// Method that stores a user session in the database.
    /// </summary>
    public void StoreUserSession()
    {
        _userRepository.StoreUserSession();
    }
    /// <summary>
    /// Metho that gets user data from the database by session id and returns it as a UserDto object.
    /// </summary>
    public UserDto GetUserDataBySession()
    {
        return _userRepository.GetUserBySession();
    }
}