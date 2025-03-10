namespace API.DTOs
{
    public class SettingsDTO
    {
        public required int settingId { get; set; }
        public required string settingName { get; set; }
        public required string settingValue { get; set; }
    }
}