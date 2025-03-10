namespace API.DTOs
{
    public class UserDTO
    {
        public int userId { get; set; }
        public int userType { get; set; }
        public string email { get; set; }
        public List<string> userPermissions { get; set; }
        public List<SettingsDTO> userSettings { get; set; }

        public UserDTO(int userId, int userType, string email, List<string> userPermissions, List<SettingsDTO> userSettings)
        {
            this.userId = userId;
            this.userType = userType;
            this.email = email;
            this.userPermissions = userPermissions;
            this.userSettings = userSettings;
        }
    }
}