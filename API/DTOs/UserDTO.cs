namespace API.Dtos;
public class UserDto(int userId, int userType, string email, List<string> userPermissions, List<SettingsDto> userSettings)
{
    public int Id { get; set; } = userId;
    public int Type { get; set; } = userType;
    public string Email { get; set; } = email;
    public List<string> Permissions { get; set; } = userPermissions;
    public List<SettingsDto> Settings { get; set; } = userSettings;
}