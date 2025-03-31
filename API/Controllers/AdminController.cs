using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpGet("GetAllUsers")]
    public ActionResult<UserDto[]> GetAllUsers()
    {
        return Ok(adminService.GetAllUsers());

    }

    [HttpGet("GetAllContentLibraries")]
    public ActionResult<LibraryDto[]> GetAllContentLibraries()
    {
        return Ok(adminService.GetAllLibraries());
    }

    [HttpDelete("DeleteUserById")]
    public ActionResult DeleteUserById(int userId)
    {
        adminService.DeleteUserById();
        return Ok();
    }

    [HttpDelete("DeleteUsersById")]
    public ActionResult DeleteUsersById([FromBody] int[] userIds)
    {
        adminService.DeleteUsersByIds();
        return Ok();
    }

    [HttpPut("UpdateUserById")]
    public ActionResult UpdateUserById(int userId, [FromBody] UserDto updatedUser)
    {
        adminService.UpdateUserById();
        return Ok();
    }

    [HttpPut("UpdateUsersById")]
    public ActionResult UpdateUsersById([FromBody] UserDto[] user)
    {
        adminService.UpdateUsersByIds();
        return Ok();
    }

    [HttpPost("CreateNewUser")]
    public ActionResult CreateNewUser([FromBody] UserDto user)
    {
        adminService.CreateNewUser();
        return Ok();
    }

    [HttpPost("CreateNewUsers")]
    public ActionResult CreateNewUsers([FromBody] UserDto[] user)
    {
        adminService.CreateNewUsers();
        return Ok();
    }
}