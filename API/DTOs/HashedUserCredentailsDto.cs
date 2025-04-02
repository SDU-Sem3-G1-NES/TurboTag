namespace API.DTOs;

public class HashedUserCredentialsDto(int userId, byte[] passwordHash, byte[] passwordSalt)
{
    public int UserId { get; set; } = userId;
    public byte[] PasswordHash { get; set; } = passwordHash;
    public byte[] PasswordSalt { get; set; } = passwordSalt;
}