namespace API.DTOs;

public class SettingsDto
{
    public SettingsDto()
    {
        Name = "";
        Value = "";
    }

    public SettingsDto(int settingId, string settingName, string settingValue)
    {
        Id = settingId;
        Name = settingName;
        Value = settingValue;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}