namespace API.DTOs
{
    public class UserDTO
    {
        public required int userId { get; set; }
        public required int userType { get; set; }
        public required string email { get; set; }
        public required List<string> userPermissions { get; set; }
        public required List<SettingsDTO> userSettings { get; set; }

    }
}