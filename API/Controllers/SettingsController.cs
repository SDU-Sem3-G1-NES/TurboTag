using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;
        
namespace API.Controllers;
        
[ApiController]
[Route("[controller]")]
public class SettingsController(ISettingsService settingsService) : ControllerBase
{
    [HttpGet("GetUserSettingsById")]
    public ActionResult<SettingsDto> GetUserSettingsById(int id)
    {
        return Ok(settingsService.GetUserSettingsById());
    }
        
    [HttpPut("UpdateUserSettingsById")]
    public ActionResult UpdateUserSettingsById([FromBody] SettingsDto[] updatedSettings)
    {
        settingsService.UpdateUserSettingsById();
        return Ok();
    }
        
    [HttpPut("UpdateUserSettingById")]
    public ActionResult UpdateUserSettingById([FromBody] SettingsDto setting)
    {
        settingsService.UpdateUserSettingById();
        return Ok();
    }
}