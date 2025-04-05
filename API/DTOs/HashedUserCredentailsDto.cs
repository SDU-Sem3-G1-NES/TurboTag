namespace API.DTOs;

public class HashedUserCredentialsDto
{
    public HashedUserCredentialsDto()
    {
        PasswordHash = [];
        PasswordSalt = [];
    }

    public HashedUserCredentialsDto(int userId, byte[] passwordHash, byte[] passwordSalt)
    {
        UserId = userId;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public int UserId { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}