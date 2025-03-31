using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;
        
namespace API.Controllers;
        
[ApiController]
[Route("[controller]")]
public class SettingsController(ISettingsService _settingsService) : ControllerBase
{
    [HttpGet("GetUserSettingsById")]
    public ActionResult<SettingsDto> GetUserSettingsById(int id)
    {
        return Ok(_settingsService.GetUserSettingsById());
    }
        
    [HttpPut("UpdateUserSettingsById")]
    public ActionResult UpdateUserSettingsById([FromBody] SettingsDto[] updatedSettings)
    {
        _settingsService.UpdateUserSettingsById();
        return Ok();
    }
        
    [HttpPut("UpdateUserSettingById")]
    public ActionResult UpdateUserSettingById([FromBody] SettingsDto setting)
    {
        _settingsService.UpdateUserSettingById();
        return Ok();
    }
}