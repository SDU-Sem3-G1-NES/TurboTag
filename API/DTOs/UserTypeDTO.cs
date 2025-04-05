namespace API.DTOs;

public class UserTypeDto
{
    public UserTypeDto()
    {
        Name = "";
        Permissions = new Dictionary<string, bool>();
    }

    public UserTypeDto(int id, string name, Dictionary<string, bool> permissions)
    {
        Id = id;
        Name = name;
        Permissions = permissions;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public Dictionary<string, bool> Permissions { get; set; }
}