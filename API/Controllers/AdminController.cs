using System.Diagnostics;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    [HttpGet("GetAllUsers")]
    public ActionResult<UserDTO[]> GetAllUsers()
    {
        return Ok(mockUsers);
    }

    [HttpGet("GetAllContentLibraries")]
    public ActionResult<LibraryDTO[]> GetAllContentLibraries()
    {
        return Ok(mockLibraries);
    }

    [HttpDelete("DeleteUserById")]
    public ActionResult DeleteUserById(int userId)
    {
        return Ok();
    }

    [HttpDelete("DeleteUsersById")]
    public ActionResult DeleteUsersById([FromBody] int[] userIds)
    {
        return Ok();
    }

    [HttpPut("UpdateUserById")]
    public ActionResult UpdateUserById(int userId, [FromBody] UserDTO updatedUser)
    {
        return Ok();
    }

    [HttpPut("UpdateUsersById")]
    public ActionResult UpdateUsersById([FromBody] UserDTO[] user) //idk how to do this one
    {
        return Ok();
    }

    [HttpPost("CreateNewUser")]
    public ActionResult CreateNewUser([FromBody] UserDTO user)
    {
        return Ok();
    }

    [HttpPost("CreateNewUsers")]
    public ActionResult CreateNewUsers([FromBody] UserDTO[] user)
    {
        return Ok();
    }

    private List<LibraryDTO> mockLibraries = new List<LibraryDTO>
    {
        new LibraryDTO(libraryId: 1, libraryName: "Library 1")
    };

    private List<UserDTO> mockUsers = new List<UserDTO>
    {
        new UserDTO(
            userId: 1,
            email: "example@example.com",
            userType: 1,
            userPermissions: new List<string>
            {
                "Permission 1",
                "Permission 2"
            },
            userSettings: new List<SettingsDTO>
            {
                new SettingsDTO(1, "Theme", "Dark")
            }
        )
    };
}