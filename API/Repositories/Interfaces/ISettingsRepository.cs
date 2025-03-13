using API.DTOs;

namespace API.Repositories.Interfaces
{
    public interface ISettingsRepository : IRepositoryBase
    {
        void AddSetting(SettingsDTO setting);
        SettingsDTO GetSettingById(int settingId);
        List<SettingsDTO> GetAllSettings();
        void UpdateSetting(SettingsDTO setting);
        void DeleteSetting(int settingId);
    }
}