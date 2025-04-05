using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;
        
namespace API.Controllers;
        
[ApiController]
[Route("[controller]")]
public class SettingsController(ISettingsService settingsService) : ControllerBase
{
    
    [HttpGet("GetAllSettings")]
    public ActionResult<IEnumerable<SettingsDto>> GetAllSettings()
    {
        return Ok(settingsService.GetAllSettings());
    }
    
    [HttpGet("GetSettingById")]
    public ActionResult<SettingsDto> GetSettingById(int settingId)
    {
        return Ok(settingsService.GetSettingById(settingId));
    }
    [HttpPost("AddSetting")]
    public ActionResult AddSetting([FromBody] SettingsDto setting)
    {
        settingsService.CreateNewSetting(setting);
        return Ok();
    }
    
    [HttpPut("UpdateSetting")]
    public ActionResult UpdateSetting([FromBody] SettingsDto updatedSetting)
    {
        settingsService.UpdateSetting(updatedSetting);
        return Ok();
    }
    
    [HttpDelete("DeleteSettingById")]
    public ActionResult DeleteSettingById(int settingId)
    {
        settingsService.DeleteSettingById(settingId);
        return Ok();
    }
}