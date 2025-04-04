using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface IAdminService : IServiceBase
{
    IEnumerable<UserDto> GetAllUsers(UserFilter? filter);
    UserDto GetUserByEmail(string email);
    void CreateNewUser(UserDto user, UserCredentialsDto userCredentials);
    void UpdateUser(UserDto user);
    void DeleteUserById(int userId);
}

public class AdminService(IUserRepository userRepository, IUserCredentialsService userCredentialsService) : IAdminService
{
    public IEnumerable<UserDto> GetAllUsers(UserFilter? filter)
    {
        return userRepository.GetAllUsers(filter);
    }

    public UserDto GetUserByEmail(string email)
    {
        return userRepository.GetUserByEmail(email);
    }

    public void CreateNewUser(UserDto user, UserCredentialsDto userCredentials)
    {
        int userId = userRepository.AddUser(user);
        userCredentialsService.AddUserCredentials(userId, userCredentials);
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