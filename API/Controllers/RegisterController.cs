using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;
namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class RegisterController(IUserCredentialsService _userCredentialsService) : ControllerBase
{
    [HttpGet("CheckIfUserExistsByEmail")]
    public ActionResult<bool> CheckIfUserExistsByEmail(string email)
    {
        return Ok(_userCredentialsService.CheckIfUserExistsByEmail());
    }
    [HttpPost("RegisterUser")]
    public ActionResult RegisterUser([FromBody] UserCredentialsDTO userCredentials)
    {
        _userCredentialsService.StoreNewUserCredentials();
        return Ok();
    }
}