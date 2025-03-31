using System.Text.Json.Serialization;

namespace API.Dtos;

[method: JsonConstructor]
public class UserCredentialsDto(string email, string password)
{
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
}