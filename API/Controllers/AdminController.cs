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
        return Ok(users);
    }

    [HttpGet("GetAllContentLibraries")]
    public ActionResult<LibraryDTO[]> GetAllContentLibraries()
    {
        return Ok(libraries);
    }

    [HttpDelete("DeleteUserById")]
    public ActionResult DeleteUserById(int userId)
    {
        var existingUser = users.FirstOrDefault(u => u.userId == userId);
        if (existingUser == null)
        {
            return NotFound();
        }
        users.Remove(existingUser);
        return Ok();
    }

    [HttpDelete("DeleteUsersById")]
    public ActionResult DeleteUsersById([FromBody] int[] userIds)
    {
        users.RemoveAll(u => userIds.Contains(u.userId));
        return Ok();
    }

    [HttpPut("UpdateUserById")]
    public ActionResult UpdateUserById(int userId, [FromBody] UserDTO updatedUser)
    {
        foreach (var user in users)
        {
            if (user.userId == userId)
            {
                user.email = updatedUser.email;
                user.userType = updatedUser.userType;
                user.userPermissions = updatedUser.userPermissions;
                user.userSettings = updatedUser.userSettings;
            }
        }
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
        users.Add(user);
        return Ok(users);
    }

    [HttpPost("CreateNewUsers")]
    public ActionResult CreateNewUsers([FromBody] UserDTO[] user)
    {
        users.AddRange(user);
        return Ok();
    }

    private List<LibraryDTO> libraries = new List<LibraryDTO>
    {
        new LibraryDTO(libraryId: 1, libraryName: "Library 1")
    };

    private List<UserDTO> users = new List<UserDTO>
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