using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController(IUserCredentialsService _userCredentialsService) : ControllerBase
{
    [HttpGet("ValidateUserCredentials")]
    public ActionResult<bool> ValidateUserCredentials([FromBody] UserCredentialsDTO userCredentials)
    {
        return Ok(_userCredentialsService.ValidateUserCredentials());
    }

    [HttpGet("GetUserDataByEmail")]
    public ActionResult<UserDTO> GetUserDataByEmail(string email)
    {
        return Ok(_userCredentialsService.GetUserDataByEmail());
    }
    [HttpPost("StoreUserSession")]
    public ActionResult StoreUserSession()
    {
        _userCredentialsService.StoreUserSession();
        return Ok();
    }
    [HttpGet("GetUserDataBySessionId")]
    public ActionResult GetUserDataBySessionId(string sessionId)
    {
        return Ok(_userCredentialsService.GetUserDataBySession());
    }
}