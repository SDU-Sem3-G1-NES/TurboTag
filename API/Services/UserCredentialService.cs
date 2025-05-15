using API.DTOs;
using API.Repositories;
using System.Security.Cryptography;

namespace API.Services;

public interface IUserCredentialService : IServiceBase
{
    bool UserExists(string email);
    void AddUserCredentials(int userId, string password);
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

    public void AddUserCredentials(int userId, string password)
    {
        var hashedUserCredentials = HashUserCredentials(userId, password);
        userRepository.AddUserCredentials(hashedUserCredentials);
    }

    public bool ValidateUserCredentials(UserCredentialsDto userCredentials)
    {
        return true;
    }

    public UserDto GetUserByEmail(string email)
    {
        return userRepository.GetUserByEmail(email) ?? new UserDto();
    }
    
    private HashedUserCredentialsDto HashUserCredentials(int userId, string password)
    {
        // Generate a random salt
        var salt = RandomNumberGenerator.GetBytes(16);
        // Hash the password with the salt
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        return new HashedUserCredentialsDto
        {
            UserId = userId,
            PasswordHash = hash,
            PasswordSalt = salt
        };
    }
}