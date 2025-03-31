using API.Dtos;
using API.Repositories;

namespace API.Services;

public interface ISettingsService : IServiceBase
{
    List<SettingsDto> GetUserSettingsById();
    void UpdateUserSettingsById();
    void UpdateUserSettingById();
}

public class SettingService(ISettingsRepository _settingsRepository) : ISettingsService
{
    /// <summary>
    /// Method that returns a User's Settings from the database by Id.
    /// </summary>
    public List<SettingsDto> GetUserSettingsById()
    {
        return _settingsRepository.GetAllSettings();
    }
    /// <summary>
    /// Method that updates a User's Settings in the database by Id.
    /// </summary>
    public void UpdateUserSettingsById()
    {
        // Either call one by one or make a method in repository for updating all settings at once.
        _settingsRepository.UpdateSetting(new SettingsDto(1, "Mock Setting", "Mock Value"));
    }
    /// <summary>
    /// Method that updates a User's Setting in the database by Id.
    /// </summary>
    public void UpdateUserSettingById()
    {
        _settingsRepository.UpdateSetting(new SettingsDto(1, "Mock Setting", "Mock Value"));
    }
}