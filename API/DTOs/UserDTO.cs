namespace API.DTOs;
public class UserDto(int id, int userTypeId, string name, string email, List<int> accessibleLibraryIds)
{
    public int Id { get; set; } = id;
    public int UserTypeId { get; set; } = userTypeId;
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public List<int> AccessibleLibraryIds { get; set; } = accessibleLibraryIds;
}