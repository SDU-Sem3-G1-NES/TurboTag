using API.Dtos;

namespace API.Repositories
{
    public interface ISettingsRepository : IRepositoryBase
    {
        int AddSetting(SettingsDto setting);
        SettingsDto GetSettingById(int settingId);
        List<SettingsDto> GetAllSettings();
        void UpdateSetting(SettingsDto setting);
        void DeleteSetting(int settingId);
    }
    public class SettingsRepository : ISettingsRepository
    {
        public int AddSetting(SettingsDto setting)
        {
            return 1;
        }
        public SettingsDto GetSettingById(int settingId)
        {
            return new SettingsDto(settingId, "Mock Setting", "Mock Value");
        }
        public List<SettingsDto> GetAllSettings()
        {
            return new List<SettingsDto>
            {
                new SettingsDto(1, "Mock Setting", "Mock Value"),
                new SettingsDto(2, "Mock Setting", "Mock Value")
            };
        }
        public void UpdateSetting(SettingsDto setting)
        {
            throw new NotImplementedException();
        }
        public void DeleteSetting(int settingId)
        {
            throw new NotImplementedException();
        }
    }
}