using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ContentLibraryController(ILibraryService _libraryService) : ControllerBase
{
    [HttpGet("GetUserLibrariesById")]
    public ActionResult<LibraryDTO[]> GetUserLibrariesById(string userId)
    {
        return Ok(_libraryService.GetUserLibrariesById());
    }
    [HttpGet("GetUserLibraryId")]
    public ActionResult<LibraryDTO> GetUserLibraryId(string libraryId)
    {
        return Ok(_libraryService.GetUserLibraryById());
    }
    [HttpGet("GetLibraryUploadsById")]
    public ActionResult<UploadDTO[]> GetLibraryUploadsById(string libraryId)
    {
        return Ok(_libraryService.GetLibraryUploadsById());
    }
    [HttpGet("GetLibraryUploadById")]
    public ActionResult<UploadDTO> GetLibraryUploadById(string uploadId)
    {
        return Ok(_libraryService.GetLibraryUploadById());
    }
}
