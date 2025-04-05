using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface ISettingsService : IServiceBase
{
    List<SettingsDto> GetAllSettings();
    SettingsDto GetSettingById(int settingId);
    void CreateNewSetting(SettingsDto setting);
    void UpdateSetting(SettingsDto setting);
    void DeleteSettingById(int settingId);
}

public class SettingService(ISettingsRepository settingsRepository) : ISettingsService
{
    public List<SettingsDto> GetAllSettings()
    {
        return settingsRepository.GetAllSettings();
    }
    
    public SettingsDto GetSettingById(int settingId)
    {
        return settingsRepository.GetSettingById(settingId);
    }
    
    public void CreateNewSetting(SettingsDto setting)
    {
        settingsRepository.AddSetting(setting);
    }
    
    public void UpdateSetting(SettingsDto setting)
    {
        settingsRepository.UpdateSetting(setting);
    }
    
    public void DeleteSettingById(int settingId)
    {
        settingsRepository.DeleteSettingById(settingId);
    }
}