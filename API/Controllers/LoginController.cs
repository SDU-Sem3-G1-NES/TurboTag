using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController(IUserCredentialsService userCredentialsService) : ControllerBase
{
    [HttpGet("ValidateUserCredentials")]
    public ActionResult<bool> ValidateUserCredentials([FromBody] UserCredentialsDto userCredentials)
    {
        return Ok(userCredentialsService.ValidateUserCredentials());
    }

    [HttpGet("GetUserDataByEmail")]
    public ActionResult<UserDto> GetUserDataByEmail(string email)
    {
        return Ok(userCredentialsService.GetUserDataByEmail());
    }
    [HttpPost("StoreUserSession")]
    public ActionResult StoreUserSession()
    {
        userCredentialsService.StoreUserSession();
        return Ok();
    }
    [HttpGet("GetUserDataBySessionId")]
    public ActionResult GetUserDataBySessionId(string sessionId)
    {
        return Ok(userCredentialsService.GetUserDataBySession());
    }
}