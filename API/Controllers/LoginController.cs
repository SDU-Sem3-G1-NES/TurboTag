using System.Diagnostics;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    [HttpGet("ValidateUserCredentials")]
    public ActionResult<bool> ValidateUserCredentials([FromBody] UserCredentialsDTO userCredentials)
    {
        if (mockuserCredentials.Any(c => c.Email == userCredentials.Email && c.Password == userCredentials.Password))
        {
            return Ok(true);
        }
        return NotFound(true);
    }

    [HttpGet("GetUserDataByEmail")]
    public ActionResult<UserDTO> GetUserDataByEmail(string email)
    {
        var existingUser = users.FirstOrDefault(u => u.email == email);
        if (existingUser == null)
        {
            return NotFound();
        }
        return Ok(existingUser);
    }
    [HttpPost("StoreUserSession")]
    public ActionResult StoreUserSession()
    {
        return Ok();
    }
    [HttpGet("GetUserDataBySessionId")]
    public ActionResult GetUserDataBySessionId(string sessionId)
    {
        return Ok();
    }

    private List<UserDTO> users = new List<UserDTO>
    {
        new UserDTO(
            userId: 1,
            userType: 1,
            email: "example@example.com",
            userPermissions: new List<string> { "read", "write" },
            userSettings: new List<SettingsDTO>
            {
                new SettingsDTO(1, "Theme", "Dark")
            }
        )
    };

    private List<UserCredentialsDTO> mockuserCredentials = new List<UserCredentialsDTO>
    {
        new UserCredentialsDTO("example@example.com", "example")
    };
}