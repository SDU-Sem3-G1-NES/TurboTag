namespace API.DTOs;

public class UserCredentialsDTO(string email, string password)
{
    public string Email { get; set; }
    public string Password { get; set; }
}