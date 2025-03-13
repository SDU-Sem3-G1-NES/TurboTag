namespace API.DTOs;
public class UserDTO(int userId, int userType, string email, List<string> userPermissions, List<SettingsDTO> userSettings)
{
    public int Id { get; } = userId;
    public int Type { get; } = userType;
    public string Email { get; } = email;
    public List<string> Permissions { get; } = userPermissions;
    public List<SettingsDTO> Settings { get; } = userSettings;
}