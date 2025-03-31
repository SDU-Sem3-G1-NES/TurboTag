namespace API.Dtos;

public class SettingsDto(int settingId, string settingName, string settingValue)
{
    public int Id { get; set; } = settingId;
    public string Name { get; set; } = settingName;
    public string Value { get; set; } = settingValue;
}