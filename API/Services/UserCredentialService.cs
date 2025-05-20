using System.Security.Cryptography;
using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface IUserCredentialService : IServiceBase
{
    bool UserExists(string email);
    void AddUserCredentials(int userId, UserCredentialsDto userCredentials);
    bool ValidateUserCredentials(int userId, string password);
    UserDto GetUserByEmail(string email);
    public void SetupCredentials(int userId, string password);
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

    public bool ValidateUserCredentials(int userId, string password)
    {
        var hashedUserCredentials = userRepository.GetUserCredentials(userId);
        
        using var pbkdf2 = new Rfc2898DeriveBytes(password, hashedUserCredentials.PasswordSalt, 100_000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);
        
        return CryptographicOperations.FixedTimeEquals(hash, hashedUserCredentials.PasswordHash);
    }

    public UserDto GetUserByEmail(string email)
    {
        return userRepository.GetUserByEmail(email) ?? new UserDto();
    }
    public void SetupCredentials(int userId, string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        
        var hashedUserCredentials = new HashedUserCredentialsDto
        {
            UserId = userId,
            PasswordHash = hash,
            PasswordSalt = salt
        };
        
        userRepository.UpdateUserCredentials(hashedUserCredentials);
    }
}