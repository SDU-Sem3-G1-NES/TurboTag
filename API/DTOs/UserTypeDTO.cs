namespace API.DTOs;

public class UserTypeDto(int id, string name, Dictionary<string, bool> permissions)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public Dictionary<string, bool> Permissions { get; set; } = permissions;
}