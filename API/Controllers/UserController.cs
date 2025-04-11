using API.DTOs;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService, IUserCredentialService userCredentialService) : ControllerBase
{
    [HttpPost("GetAllUsers")]
    public ActionResult<IEnumerable<UserDto>> GetAllUsers([FromBody] UserFilter? filter)
    {
        return Ok(userService.GetAllUsers(filter));
    }

    [HttpGet("GetUserByEmail")]
    public ActionResult<UserDto> GetUserByEmail(string email)
    {
        return Ok(userService.GetUserByEmail(email));
    }

    [HttpPost("CreateNewUser")]
    public ActionResult CreateNewUser([FromBody] (UserDto user, UserCredentialsDto userCredentials) parameters)
    {
        userService.CreateNewUser(parameters.user, parameters.userCredentials);
        return Ok();
    }

    [HttpPut("UpdateUserById")]
    public ActionResult UpdateUserById([FromBody] UserDto updatedUser)
    {
        userService.UpdateUser(updatedUser);
        return Ok();
    }

    [HttpDelete("DeleteUserById")]
    public ActionResult DeleteUserById(int userId)
    {
        userService.DeleteUserById(userId);
        return Ok();
    }

    [HttpGet("UserExists")]
    public ActionResult<bool> UserExists(string email)
    {
        return Ok(userCredentialService.UserExists(email));
    }
}