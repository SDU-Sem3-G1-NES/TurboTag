using API.DTOs;
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
}