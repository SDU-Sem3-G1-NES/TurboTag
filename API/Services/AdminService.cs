using API.Dtos;
using API.Repositories;

namespace API.Services;
public interface IAdminService : IServiceBase
{
    List<UserDto> GetAllUsers();
    UserDto GetUserById();
    List<LibraryDto> GetAllLibraries();
    LibraryDto GetLibraryById();
    void CreateNewUser();
    void CreateNewUsers();
    void UpdateUserById();
    void UpdateUsersByIds();
    void DeleteUserById();
    void DeleteUsersByIds();
}
public class AdminService(IUserRepository _userRepository, ILibraryRepository _libraryRepository) : IAdminService
{
    /// <summary>
    /// Method that returns a list of all Users from the database.    
    /// </summary>
    public List<UserDto> GetAllUsers()
    {
        return _userRepository.GetAllUsers();
    }
    /// <summary>
    /// Method that returns a User from the database by Id.
    /// </summary>
    public UserDto GetUserById()
    {
        return _userRepository.GetUserById(1);
    }
    /// <summary>
    /// Method that returns a list of all Libraries from the database.
    /// </summary>
    public List<LibraryDto> GetAllLibraries()
    {
        return _libraryRepository.GetAllLibraries();
    }
    /// <summary>
    /// Method that returns a Library from the database by Id.
    /// </summary>
    public LibraryDto GetLibraryById()
    {
        return _libraryRepository.GetLibraryById(1);
    }
    /// <summary>
    /// Method that creates a new User in the database.
    /// </summary>
    public void CreateNewUser()
    {
        _userRepository.AddUser(new UserDto(1, 1, "mock@email.com", new List<string> { "Permission1", "Permission2" }, new List<SettingsDto>()));
        _userRepository.AddUserCredentials(new byte[] { 0x00, 0x01, 0x02, 0x03 }, new byte[] { 0x00, 0x01, 0x02, 0x03 });
    }
    /// <summary>
    /// Method that creates multiple new Users in the database.
    /// </summary>
    public void CreateNewUsers()
    {
        // No method in repository to add multiple users, so ig call the method for each user.
        _userRepository.AddUser(new UserDto(1, 1, "mock@mock.com", new List<string> { "Permission1", "Permission2" }, new List<SettingsDto>()));
    }
    /// <summary>
    /// Method that updates a User in the database by Id.
    /// </summary>
    public void UpdateUserById()
    {
        _userRepository.UpdateUser(new UserDto(1, 1, "mock@mock.com", new List<string> { "Permission1", "Permission2" }, new List<SettingsDto>()));
    }
    /// <summary>
    /// Method that updates multiple Users in the database by Ids.
    /// </summary>
    public void UpdateUsersByIds()
    {   
        // No method in repository to update multiple users, so ig call the method for each user.
        _userRepository.UpdateUser(new UserDto(1, 1, "mock@mock.com", new List<string> { "Permission1", "Permission2" }, new List<SettingsDto>()));
    }
    /// <summary>
    /// Method that deletes a User in the database by Id.
    /// </summary>
    public void DeleteUserById()
    {
        _userRepository.DeleteUser(1);
        _userRepository.DeleteUserCredentials(new byte[] { 0x00, 0x01, 0x02, 0x03 });
    }
    /// <summary>
    /// Method that deletes multiple Users in the database by Ids.
    /// </summary>
    public void DeleteUsersByIds()
    {
        // No method in repository to delete multiple users, so ig call the method for each user.
        _userRepository.DeleteUser(1);
        _userRepository.DeleteUserCredentials(new byte[] { 0x00, 0x01, 0x02, 0x03 });
    }
}