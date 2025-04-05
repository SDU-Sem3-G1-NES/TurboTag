using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;
namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class RegisterController(IUserCredentialsService userCredentialsService) : ControllerBase
{
    [HttpGet("CheckIfUserExistsByEmail")]
    public ActionResult<bool> CheckIfUserExistsByEmail(string email)
    {
        return Ok(userCredentialsService.CheckIfUserExistsByEmail(email));
    }
}