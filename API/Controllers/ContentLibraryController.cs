using API.DTOs;
using API.Services;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ContentLibraryController(ILibraryService libraryService) : ControllerBase
{
    [HttpPost("GetAllLibraries")]
    public ActionResult<IEnumerable<LibraryDto>> GetAllLibraries([FromBody] LibraryFilter? filter)
    {
        return Ok(libraryService.GetAllLibraries(filter));
    }

    [HttpPost("GetUserLibraries")]
    public ActionResult<IEnumerable<LibraryDto>> GetUserLibraries([FromBody] (UserDto user, LibraryFilter filter) parameters)
    {
        return Ok(libraryService.GetLibrariesByUser(parameters.user, parameters.filter));
    }
    
    [HttpGet("GetLibraryById")]
    public ActionResult<LibraryDto> GetLibraryById(int libraryId)
    {
        return Ok(libraryService.GetLibraryById(libraryId));
    }
    
    [HttpPost("CreateNewLibrary")]
    public ActionResult CreateNewLibrary([FromBody] LibraryDto library)
    {
        libraryService.CreateNewLibrary(library);
        return Ok();
    }
    
    [HttpPut("UpdateLibraryById")]
    public ActionResult UpdateLibraryById([FromBody] LibraryDto updatedLibrary)
    {
        libraryService.UpdateLibrary(updatedLibrary);
        return Ok();
    }
    
    [HttpDelete("DeleteLibraryById")]
    public ActionResult DeleteLibraryById(int libraryId)
    {
        libraryService.DeleteLibraryById(libraryId);
        return Ok();
    }
}