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
        return Ok(mockSettings.FirstOrDefault(s => s.Id == id));
    }
        
    [HttpPut("UpdateUserSettingsById")]
    public ActionResult UpdateUserSettingsById([FromBody] SettingsDTO[] updatedSettings)
    {
        foreach (var updatedSetting in updatedSettings)
        {
            var existingSetting = mockSettings.FirstOrDefault(s => s.Id == updatedSetting.Id);
            if (existingSetting != null)
            {
                mockSettings.Remove(existingSetting);
                mockSettings.Add(updatedSetting);
            }
        }
        return Ok();
    }
        
    [HttpPut("UpdateUserSettingById")]
    public ActionResult UpdateUserSettingById([FromBody] SettingsDTO setting)
    {
        var existingSetting = mockSettings.FirstOrDefault(s => s.Id == setting.Id);
        if (existingSetting == null)
        {
            return NotFound();
        }
        mockSettings.Remove(existingSetting);
        mockSettings.Add(setting);
        return Ok();
    }
    
    private List<SettingsDTO> mockSettings = new List<SettingsDTO>
    {
        new SettingsDTO(1, "Theme", "Dark")
    };
}