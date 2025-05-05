using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController(IUserCredentialService userCredentialService) : ControllerBase
{
    [HttpGet("ValidateUserCredentials")]
    public ActionResult<bool> ValidateUserCredentials([FromBody] UserCredentialsDto userCredentials)
    {
        return Ok(userCredentialService.ValidateUserCredentials(userCredentials));
    }

    [HttpGet("GetUserDataByEmail")]
    public ActionResult<UserDto> GetUserDataByEmail(string email)
    {
        return Ok(userCredentialService.GetUserByEmail(email));
    }
}