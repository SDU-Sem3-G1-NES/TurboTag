namespace API.DTOs;

public class UserDto
{
    public UserDto()
    {
        Name = "";
        Email = "";
        AccessibleLibraryIds = new List<int>();
    }

    public UserDto(int id, int userTypeId, string name, string email, List<int> accessibleLibraryIds)
    {
        Id = id;
        UserTypeId = userTypeId;
        Name = name;
        Email = email;
        AccessibleLibraryIds = accessibleLibraryIds;
    }

    public int Id { get; set; }
    public int UserTypeId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<int> AccessibleLibraryIds { get; set; }
}