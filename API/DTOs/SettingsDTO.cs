using YamlDotNet.Serialization;

namespace API.DTOs
{
    public class SettingsDTO
    {
        public int settingId { get; set; }
        public string settingName { get; set; }
        public string settingValue { get; set; }
        public SettingsDTO(int settingId, string settingName, string settingValue)
        {
            this.settingId = settingId;
            this.settingName = settingName;
            this.settingValue = settingValue;
        }
    }
}