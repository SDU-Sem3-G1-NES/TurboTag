using API.DTOs;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
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
    public ActionResult CreateNewUser([FromBody] UserRequest request)
    {
        userService.CreateNewUser(request.User, request.Password);
        return Ok();
    }

    [HttpPut("UpdateUserById")]
    public ActionResult UpdateUserById([FromBody] UserRequest request)
    {
        userService.UpdateUser(request.User, request.Password);
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
    
    public class UserRequest
    {
        public UserDto User { get; set; }
        public string? Password { get; set; }
    }
}