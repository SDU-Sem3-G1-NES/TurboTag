using YamlDotNet.Serialization;

namespace API.DTOs
{
    public class SettingsDTO
    {
        public required int settingId { get; set; }
        public required string settingName { get; set; }
        public required string settingValue { get; set; }
        public SettingsDTO(int settingId, string settingName, string settingValue)
        {
            this.settingId = settingId;
            this.settingName = settingName;
            this.settingValue = settingValue;
        }
    }
}