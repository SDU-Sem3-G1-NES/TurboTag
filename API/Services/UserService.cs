using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface IUserService : IServiceBase
{
    IEnumerable<UserDto> GetAllUsers(UserFilter? filter);
    UserDto GetUserByEmail(string email);
    UserDto GetUserById(int id);
    void CreateNewUser(UserDto user, UserCredentialsDto userCredentials);
    void UpdateUser(UserDto user);
    void DeleteUserById(int userId);
}

public class UserService(IUserRepository userRepository, IUserCredentialService userCredentialService) : IUserService
{
    public IEnumerable<UserDto> GetAllUsers(UserFilter? filter)
    {
        return userRepository.GetAllUsers(filter);
    }

    public UserDto GetUserByEmail(string email)
    {
        return userRepository.GetUserByEmail(email);
    }

    public UserDto GetUserById(int id)
    {
        return userRepository.GetUserById(id);
    }

    public void CreateNewUser(UserDto user, UserCredentialsDto userCredentials)
    {
        var userId = userRepository.AddUser(user);
        userCredentialService.AddUserCredentials(userId, userCredentials);
    }

    public void UpdateUser(UserDto user)
    {
        userRepository.UpdateUser(user);
    }

    public void DeleteUserById(int userId)
    {
        userRepository.DeleteUserById(userId);
    }
}