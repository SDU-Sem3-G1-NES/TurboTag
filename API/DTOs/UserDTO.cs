namespace API.DTOs;

public class UserDto
{
    public UserDto()
    {
        Name = "";
        Email = "";
    }

    public UserDto(int id, int userTypeId, string name, string email)
    {
        Id = id;
        UserTypeId = userTypeId;
        Name = name;
        Email = email;
    }

    public int Id { get; set; }
    public int UserTypeId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}