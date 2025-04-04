using Microsoft.AspNetCore.Mvc;
using API.Dtos;
using API.Services;
namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class RegisterController(IUserCredentialsService userCredentialsService) : ControllerBase
{
    [HttpGet("CheckIfUserExistsByEmail")]
    public ActionResult<bool> CheckIfUserExistsByEmail(string email)
    {
        return Ok(userCredentialsService.CheckIfUserExistsByEmail());
    }
    [HttpPost("RegisterUser")]
    public ActionResult RegisterUser([FromBody] UserCredentialsDto userCredentials)
    {
        userCredentialsService.StoreNewUserCredentials();
        return Ok();
    }
}