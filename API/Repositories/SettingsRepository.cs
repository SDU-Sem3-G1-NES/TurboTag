using API.DTOs;

namespace API.Repositories
{
    public interface ISettingsRepository : IRepositoryBase
    {
        void AddSetting(SettingsDTO setting);
        SettingsDTO GetSettingById(int settingId);
        List<SettingsDTO> GetAllSettings();
        void UpdateSetting(SettingsDTO setting);
        void DeleteSetting(int settingId);
    }
    public class SettingsRepository : ISettingsRepository
    {
        public void AddSetting(SettingsDTO setting)
        {
            throw new NotImplementedException();
        }
        public SettingsDTO GetSettingById(int settingId)
        {
            return new SettingsDTO(settingId, "Mock Setting", "Mock Value");
        }
        public List<SettingsDTO> GetAllSettings()
        {
            return new List<SettingsDTO> { new SettingsDTO(1, "Mock Setting", "Mock Value"), new SettingsDTO(2, "Mock Setting", "Mock Value") };
        }
        public void UpdateSetting(SettingsDTO setting)
        {
            throw new NotImplementedException();
        }
        public void DeleteSetting(int settingId)
        {
            throw new NotImplementedException();
        }
    }
}