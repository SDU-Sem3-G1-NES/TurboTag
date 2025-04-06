using API.DTOs;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpPost("GetAllUsers")]
    public ActionResult<IEnumerable<UserDto>> GetAllUsers([FromBody] UserFilter? filter)
    {
        return Ok(adminService.GetAllUsers(filter));
    }
    [HttpGet("GetUserByEmail")]
    public ActionResult<UserDto> GetUserByEmail(string email)
    {
        return Ok(adminService.GetUserByEmail(email));
    }
    
    [HttpPost("CreateNewUser")]
    public ActionResult CreateNewUser([FromBody] (UserDto user, UserCredentialsDto userCredentials) parameters)
    {
        adminService.CreateNewUser(parameters.user, parameters.userCredentials);
        return Ok();
    }

    [HttpPut("UpdateUserById")]
    public ActionResult UpdateUserById([FromBody] UserDto updatedUser)
    {
        adminService.UpdateUser(updatedUser);
        return Ok();
    }
    
    [HttpDelete("DeleteUserById")]
    public ActionResult DeleteUserById(int userId)
    {
        adminService.DeleteUserById(userId);
        return Ok();
    }
}