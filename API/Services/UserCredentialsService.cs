using API.DTOs;
using API.Repositories;

namespace API.Services;
public interface IUserCredentialsService : IServiceBase
{
    bool CheckIfUserExistsByEmail();
    void StoreNewUserCredentials();
    bool ValidateUserCredentials();
    UserDTO GetUserDataByEmail();
    void StoreUserSession();
    UserDTO GetUserDataBySession();
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
    /// Method that gets user data from the database and returns it as a UserDTO object.
    /// </summary>
    public UserDTO GetUserDataByEmail()
    {
        // missing method in user repository. Get user by email.
        return _userRepository.GetUserById(1);
    }
    /// <summary>
    /// Method that stores a user session in the database.
    /// </summary>
    public void StoreUserSession()
    {
        // missing repository method. Store user session.
    }
    /// <summary>
    /// Metho that gets user data from the database by session id and returns it as a UserDTO object.
    /// </summary>
    public UserDTO GetUserDataBySession()
    {
        // missing repository method. Get user by session.
        return _userRepository.GetUserById(1);
    }
}