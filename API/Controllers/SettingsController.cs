using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
        
namespace API.Controllers;
        
[ApiController]
[Route("[controller]")]
public class SettingsController : ControllerBase
{
    [HttpGet("GetUserSettingsById")]
    public ActionResult<SettingsDTO> GetUserSettingsById(int id)
    {
        return Ok(settings.FirstOrDefault(s => s.settingId == id));
    }
        
    [HttpPut("UpdateUserSettingsById")]
    public ActionResult UpdateUserSettingsById([FromBody] SettingsDTO[] updatedSettings)
    {
        foreach (var updatedSetting in updatedSettings)
        {
            var existingSetting = settings.FirstOrDefault(s => s.settingId == updatedSetting.settingId);
            if (existingSetting != null)
            {
                existingSetting.settingValue = updatedSetting.settingValue;
            }
        }
        return Ok();
    }
        
    [HttpPut("UpdateUserSettingById")]
    public ActionResult UpdateUserSettingById([FromBody] SettingsDTO setting)
    {
        var existingSetting = settings.FirstOrDefault(s => s.settingId == setting.settingId);
        if (existingSetting == null)
        {
            return NotFound();
        }
        existingSetting.settingValue = setting.settingValue;
        return Ok();
    }
    
    private List<SettingsDTO> settings = new List<SettingsDTO>
    {
        new SettingsDTO(1, "Theme", "Dark")
    };
}