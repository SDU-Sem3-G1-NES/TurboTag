using API.DTOs;
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
public class AdminService(IUserRepository userRepository, ILibraryRepository libraryRepository) : IAdminService
{
    /// <summary>
    /// Method that returns a list of all Users from the database.    
    /// </summary>
    public List<UserDto> GetAllUsers()
    {
        return userRepository.GetAllUsers(null).Items;
    }
    /// <summary>
    /// Method that returns a User from the database by Id.
    /// </summary>
    public UserDto GetUserById()
    {
        return userRepository.GetUserById(1);
    }
    /// <summary>
    /// Method that returns a list of all Libraries from the database.
    /// </summary>
    public List<LibraryDto> GetAllLibraries()
    {
        return libraryRepository.GetAllLibraries(null).Items;
    }
    /// <summary>
    /// Method that returns a Library from the database by Id.
    /// </summary>
    public LibraryDto GetLibraryById()
    {
        return libraryRepository.GetLibraryById(1);
    }
    /// <summary>
    /// Method that creates a new User in the database.
    /// </summary>
    public void CreateNewUser()
    {
        userRepository.AddUser(new UserDto(1, 1, "John Doe", "mock@email.com", new List<int>{ 1, 2} ));
        userRepository.AddUserCredentials(1, new byte[] { 0x00, 0x01, 0x02, 0x03 }, new byte[] { 0x00, 0x01, 0x02, 0x03 });
    }
    /// <summary>
    /// Method that creates multiple new Users in the database.
    /// </summary>
    public void CreateNewUsers()
    {
        // No method in repository to add multiple users, so ig call the method for each user.
        userRepository.AddUser(new UserDto(1, 1, "John Doe", "mock@email.com", new List<int>{ 1, 2} ));
    }
    /// <summary>
    /// Method that updates a User in the database by Id.
    /// </summary>
    public void UpdateUserById()
    {
        userRepository.UpdateUser(new UserDto(1, 1, "John Doe", "mock@email.com", new List<int>{ 1, 2} ));
    }
    /// <summary>
    /// Method that updates multiple Users in the database by Ids.
    /// </summary>
    public void UpdateUsersByIds()
    {   
        // No method in repository to update multiple users, so ig call the method for each user.
        userRepository.UpdateUser(new UserDto(1, 1, "John Doe", "mock@email.com", new List<int>{ 1, 2} ));
    }
    /// <summary>
    /// Method that deletes a User in the database by Id.
    /// </summary>
    public void DeleteUserById()
    {
        userRepository.DeleteUser(1);
    }
    /// <summary>
    /// Method that deletes multiple Users in the database by Ids.
    /// </summary>
    public void DeleteUsersByIds()
    {
        // No method in repository to delete multiple users, so ig call the method for each user.
        userRepository.DeleteUser(1);
    }
}