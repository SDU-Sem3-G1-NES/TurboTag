namespace API.DTOs;
public class SettingsDTO(int settingId, string settingName, string settingValue)
{
    public int Id { get; } = settingId;
    public string Name { get; } = settingName;
    public string Value { get; } = settingValue;
}