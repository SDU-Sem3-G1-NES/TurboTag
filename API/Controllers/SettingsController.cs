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
        return Ok(mockSettings.First());
    }
        
    [HttpPut("UpdateUserSettingsById")]
    public ActionResult UpdateUserSettingsById([FromBody] SettingsDTO[] updatedSettings)
    {
        return Ok();
    }
        
    [HttpPut("UpdateUserSettingById")]
    public ActionResult UpdateUserSettingById([FromBody] SettingsDTO setting)
    {
        return Ok();
    }
    
    private List<SettingsDTO> mockSettings = new List<SettingsDTO>
    {
        new SettingsDTO(1, "Theme", "Dark")
    };
}