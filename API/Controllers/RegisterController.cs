using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class RegisterController : ControllerBase
{
    [HttpGet("CheckIfUserExistsByEmail")]
    public ActionResult<bool> CheckIfUserExistsByEmail(string email)
    {
        return Ok(true);
    }
    [HttpPost("RegisterUser")]
    public ActionResult RegisterUser([FromBody] UserCredentialsDTO userCredentials)
    {
        return Ok();
    }
}