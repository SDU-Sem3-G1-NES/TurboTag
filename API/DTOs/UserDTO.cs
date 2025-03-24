namespace API.DTOs;
public class UserDTO(int userId, int userType, string email, List<string> userPermissions, List<SettingsDTO> userSettings)
{
    public int Id { get; set; } = userId;
    public int Type { get; set; } = userType;
    public string Email { get; set; } = email;
    public List<string> Permissions { get; set; } = userPermissions;
    public List<SettingsDTO> Settings { get; set; } = userSettings;
}