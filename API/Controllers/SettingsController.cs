using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;
        
namespace API.Controllers;
        
[ApiController]
[Route("[controller]")]
public class SettingsController(ISettingsService _settingsService) : ControllerBase
{
    [HttpGet("GetUserSettingsById")]
    public ActionResult<SettingsDTO> GetUserSettingsById(int id)
    {
        return Ok(_settingsService.GetUserSettingsById());
    }
        
    [HttpPut("UpdateUserSettingsById")]
    public ActionResult UpdateUserSettingsById([FromBody] SettingsDTO[] updatedSettings)
    {
        _settingsService.UpdateUserSettingsById();
        return Ok();
    }
        
    [HttpPut("UpdateUserSettingById")]
    public ActionResult UpdateUserSettingById([FromBody] SettingsDTO setting)
    {
        _settingsService.UpdateUserSettingById();
        return Ok();
    }
}