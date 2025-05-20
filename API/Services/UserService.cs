using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface IUserService : IServiceBase
{
    IEnumerable<UserDto> GetAllUsers(UserFilter? filter);
    UserDto GetUserByEmail(string email);
    UserDto GetUserById(int id);
    void CreateNewUser(UserDto user, string password);
    void UpdateUser(UserDto user, string? password);
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
        return userRepository.GetUserByEmail(email) ?? new UserDto();
    }

    public UserDto GetUserById(int id)
    {
        return userRepository.GetUserById(id);
    }

    public void CreateNewUser(UserDto user, string password)
    {
        var userId = userRepository.AddUser(user);
        userCredentialService.AddUserCredentials(userId, password);
    }

    public void UpdateUser(UserDto user, string? password)
    {
        userRepository.UpdateUser(user);
        if (!string.IsNullOrEmpty(password))
        {
            userCredentialService.UpdateUserCredentials(user.Id, password);
        }
    }

    public void DeleteUserById(int userId)
    {
        userRepository.DeleteUserById(userId);
    }
}