using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface IUserCredentialService : IServiceBase
{
    bool UserExists(string email);
    void AddUserCredentials(int userId, UserCredentialsDto userCredentials);
    bool ValidateUserCredentials(UserCredentialsDto userCredentials);
    UserDto GetUserByEmail(string email);
}

public class UserCredentialService(IUserRepository userRepository) : IUserCredentialService
{
    public bool UserExists(string email)
    {
        var user = userRepository.GetUserByEmail(email);
        return user != null;
    }

    public void AddUserCredentials(int userId, UserCredentialsDto userCredentials)
    {
        var hashedUserCredentials = new HashedUserCredentialsDto();
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