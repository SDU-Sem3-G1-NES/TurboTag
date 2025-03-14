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
        var existingUser = mockuserCredentials.FirstOrDefault(u => u.Email == email);
        if (existingUser == null)
        {
            return NotFound(false);
        }
        return Ok(true);
    }
    [HttpPost("RegisterUser")]
    public ActionResult RegisterUser([FromBody] UserCredentialsDTO userCredentials)
    {
        if (mockuserCredentials.Any(u => u.Email == userCredentials.Email))
        {
            return Conflict();
        }
        mockuserCredentials.Add(userCredentials);
        return Ok();
    }
    
    private List<UserCredentialsDTO> mockuserCredentials = new List<UserCredentialsDTO>
    {
        new UserCredentialsDTO("example@example.com", "example")
    };
}