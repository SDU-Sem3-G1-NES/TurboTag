using API.DTOs;
using API.Repositories;

namespace API.Services;
public interface IUserCredentialsService : IServiceBase
{
    bool CheckIfUserExistsByEmail(string email);
    void AddUserCredentials(int userId, UserCredentialsDto userCredentials);
    bool ValidateUserCredentials(UserCredentialsDto userCredentials);
    UserDto GetUserByEmail(string email);
}
public class UserCredentialsService(IUserRepository userRepository) : IUserCredentialsService
{
    public bool CheckIfUserExistsByEmail(string email)
    {
        return userRepository.UserEmailExists(email);
    }

    public void AddUserCredentials(int userId, UserCredentialsDto userCredentials)
    {
        HashedUserCredentialsDto hashedUserCredentials = new HashedUserCredentialsDto();
        userRepository.AddUserCredentials(hashedUserCredentials);
    }

    public bool ValidateUserCredentials(UserCredentialsDto userCredentials)
    {
        return true;
    }

    public UserDto GetUserByEmail(string email)
    {
        return userRepository.GetUserByEmail(email);
    }
}