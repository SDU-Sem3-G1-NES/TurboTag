using System.Text.Json.Serialization;

namespace API.Dtos;

public class UserCredentialsDto
{
    public UserCredentialsDto()
    {
        Email = "";
        Password = "";
    }

    [method: JsonConstructor]
    public UserCredentialsDto(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public string Email { get; set; }
    public string Password { get; set; }
}