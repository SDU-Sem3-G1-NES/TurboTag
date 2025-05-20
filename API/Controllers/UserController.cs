using API.DTOs;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService, IUserCredentialService userCredentialService) : ControllerBase
{
    [Authorize(Roles = "1")]
    [HttpPost("GetAllUsers")]
    public ActionResult<IEnumerable<UserDto>> GetAllUsers([FromBody] UserFilter? filter)
    {
        return Ok(userService.GetAllUsers(filter));
    }
    
    [Authorize]
    [HttpPost("GetUserNames")]
    public ActionResult<IEnumerable<UserName>> GetUserNames([FromBody] UserFilter? filter)
    {
        var result = userService.GetAllUsers(filter).Select(u => new UserName
        {
            Id = u.Id,
            Name = u.Name
        });
        return Ok(result);
    }
    [Authorize(Roles = "1")]
    [HttpGet("GetUserByEmail")]
    public ActionResult<UserDto> GetUserByEmail(string email)
    {
        return Ok(userService.GetUserByEmail(email));
    }

    [Authorize(Roles = "1")]
    [HttpPost("CreateNewUser")]
    public ActionResult CreateNewUser([FromBody] UserRequest request)
    {
        userService.CreateNewUser(request.User, request.Password!);
        return Ok();
    }

    [Authorize(Roles = "1")]
    [HttpPut("UpdateUserById")]
    public ActionResult UpdateUserById([FromBody] UserRequest request)
    {
        userService.UpdateUser(request.User, request.Password);
        return Ok();
    }

    [Authorize(Roles = "1")]
    [HttpDelete("DeleteUserById")]
    public ActionResult DeleteUserById(int userId)
    {
        userService.DeleteUserById(userId);
        return Ok();
    }

    [Authorize]
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
    
    public class UserName
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}