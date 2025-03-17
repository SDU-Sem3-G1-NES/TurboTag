using API.DTOs;

namespace API.Services;

public interface ISettingsService : IServiceBase
{
    SettingsDTO GetUserSettingsById();
    void UpdateUserSettingsById();
    void UpdateUserSettingById();
}

public class SettingService : ISettingsService
{
    /// <summary>
    /// Method that returns a User's Settings from the database by Id.
    /// </summary>
    public SettingsDTO GetUserSettingsById()
    {
        return new SettingsDTO(1, "mockSetting", "mockValue");
    }
    /// <summary>
    /// Method that updates a User's Settings in the database by Id.
    /// </summary>
    public void UpdateUserSettingsById()
    {
    }
    /// <summary>
    /// Method that updates a User's Setting in the database by Id.
    /// </summary>
    public void UpdateUserSettingById()
    {
    }
}