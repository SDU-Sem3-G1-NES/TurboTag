using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController(IAdminService _adminService) : ControllerBase
{
    [HttpGet("GetAllUsers")]
    public ActionResult<UserDTO[]> GetAllUsers()
    {
        return Ok(_adminService.GetAllUsers());

    }

    [HttpGet("GetAllContentLibraries")]
    public ActionResult<LibraryDTO[]> GetAllContentLibraries()
    {
        return Ok(_adminService.GetAllLibraries());
    }

    [HttpDelete("DeleteUserById")]
    public ActionResult DeleteUserById(int userId)
    {
        _adminService.DeleteUserById();
        return Ok();
    }

    [HttpDelete("DeleteUsersById")]
    public ActionResult DeleteUsersById([FromBody] int[] userIds)
    {
        _adminService.DeleteUsersByIds();
        return Ok();
    }

    [HttpPut("UpdateUserById")]
    public ActionResult UpdateUserById(int userId, [FromBody] UserDTO updatedUser)
    {
        _adminService.UpdateUserById();
        return Ok();
    }

    [HttpPut("UpdateUsersById")]
    public ActionResult UpdateUsersById([FromBody] UserDTO[] user) //idk how to do this one
    {
        _adminService.UpdateUsersByIds();
        return Ok();
    }

    [HttpPost("CreateNewUser")]
    public ActionResult CreateNewUser([FromBody] UserDTO user)
    {
        _adminService.CreateNewUser();
        return Ok();
    }

    [HttpPost("CreateNewUsers")]
    public ActionResult CreateNewUsers([FromBody] UserDTO[] user)
    {
        _adminService.CreateNewUsers();
        return Ok();
    }
}