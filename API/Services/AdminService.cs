using API.DTOs;

namespace API.Services;
public interface IAdminService : IServiceBase
    {
        List<UserDTO> GetAllUsers();
        UserDTO GetUserById();
        List<LibraryDTO> GetAllLibraries();
        LibraryDTO GetLibraryById();
        void CreateNewUser();
        void CreateNewUsers();
        void UpdateUserById();
        void UpdateUsersByIds();
        void DeleteUserById();
        void DeleteUsersByIds();
    }
public class AdminService : IAdminService
{
    /// <summary>
    /// Method that returns a list of all Users from the database.    
    /// </summary>
    public List<UserDTO> GetAllUsers()
    {
        return new List<UserDTO>()
        {
            new UserDTO(1, 1, "mock@mock.com", new List<string> { "mockPermission" }, new List<SettingsDTO> { new SettingsDTO(1, "mockSetting", "mockValue") }),
            new UserDTO(2, 1, "mock2@mock.com", new List<string> { "mockPermission" }, new List<SettingsDTO> { new SettingsDTO(1, "mockSetting", "mockValue") })
        };
    }
    /// <summary>
    /// Method that returns a User from the database by Id.
    /// </summary>
    public UserDTO GetUserById()
    {
        return new UserDTO(1, 1, "mock@mock.com", new List<string> { "mockPermission" }, new List<SettingsDTO> { new SettingsDTO(1, "mockSetting", "mockValue") });
    }
    /// <summary>
    /// Method that returns a list of all Libraries from the database.
    /// </summary>
    public List<LibraryDTO> GetAllLibraries()
    {
        return new List<LibraryDTO>()
        {
            new LibraryDTO(1, "mockLibrary"),
            new LibraryDTO(2, "mockLibrary2")
        };
    }
    /// <summary>
    /// Method that returns a Library from the database by Id.
    /// </summary>
    public LibraryDTO GetLibraryById()
    {
        return new LibraryDTO(1, "mockLibrary");
    }
    /// <summary>
    /// Method that creates a new User in the database.
    /// </summary>
    public void CreateNewUser()
    {
    }
    /// <summary>
    /// Method that creates multiple new Users in the database.
    /// </summary>
    public void CreateNewUsers()
    {
    }
    /// <summary>
    /// Method that updates a User in the database by Id.
    /// </summary>
    public void UpdateUserById()
    {
    }
    /// <summary>
    /// Method that updates multiple Users in the database by Ids.
    /// </summary>
    public void UpdateUsersByIds()
    {
    }
    /// <summary>
    /// Method that deletes a User in the database by Id.
    /// </summary>
    public void DeleteUserById()
    {
    }
    /// <summary>
    /// Method that deletes multiple Users in the database by Ids.
    /// </summary>
    public void DeleteUsersByIds()
    {
    }
}