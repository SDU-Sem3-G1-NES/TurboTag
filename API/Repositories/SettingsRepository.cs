using API.DTOs;
using API.DataAccess;

namespace API.Repositories;

public interface ISettingsRepository : IRepositoryBase
{
    int AddSetting(SettingsDto setting);
    SettingsDto GetSettingById(int settingId);
    List<SettingsDto> GetAllSettings();
    void UpdateSetting(SettingsDto setting);
    void DeleteSettingById(int settingId);
}
public class SettingsRepository(ISqlDbAccess sqlDbAccess) : ISettingsRepository
{
    private readonly string _databaseName = "blank";
    
    public int AddSetting(SettingsDto setting)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@settingName", setting.Name },
            { "@settingValue", setting.Value }
        };

        var insertSql = @"
            INSERT INTO settings (setting_name, setting_value)
            VALUES (@settingName, @settingValue)
            RETURNING setting_id;";

        var settingId = sqlDbAccess.ExecuteQuery<int>(
            _databaseName,
            insertSql,
            "",
            "",
            parameters).FirstOrDefault();

        return settingId;
    }
    public SettingsDto GetSettingById(int settingId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@settingId", settingId }
        };

        var selectSql = @"
            SELECT
                setting_id as Id,
                setting_name as Name,
                setting_value as Value
            FROM settings
            WHERE setting_id = @settingId";

        var result = sqlDbAccess.ExecuteQuery<SettingsDto>(
            _databaseName,
            selectSql,
            "",
            "",
            parameters).FirstOrDefault();

        if (result == null)
        {
            throw new InvalidOperationException($"Setting with ID {settingId} not found");
        }

        return new SettingsDto(result.Id, result.Name, result.Value);
    }
    public List<SettingsDto> GetAllSettings()
    {
        var selectSql = @"
            SELECT
                setting_id as Id,
                setting_name as Name,
                setting_value as Value
            FROM settings";

        var results = sqlDbAccess.ExecuteQuery<SettingsDto>(
            _databaseName,
            selectSql,
            "",
            "",
            new Dictionary<string, object>()).ToList();

        return results.Select(r => new SettingsDto(r.Id, r.Name, r.Value)).ToList();
    }
    public void UpdateSetting(SettingsDto setting)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@settingId", setting.Id },
            { "@settingName", setting.Name },
            { "@settingValue", setting.Value }
        };

        var updateSql = @"
            UPDATE settings
            SET setting_name = @settingName,
                setting_value = @settingValue
            WHERE setting_id = @settingId";

        sqlDbAccess.ExecuteNonQuery(
            _databaseName,
            updateSql,
            parameters);
    }
    public void DeleteSettingById(int settingId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@settingId", settingId }
        };

        var deleteSql = @"
            DELETE FROM settings
            WHERE setting_id = @settingId";

        sqlDbAccess.ExecuteNonQuery(
            _databaseName,
            deleteSql,
            parameters);
    }
}