using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ContentLibraryController(ILibraryService _libraryService) : ControllerBase
{
    [HttpGet("GetUserLibrariesById")]
    public ActionResult<LibraryDto[]> GetUserLibrariesById(string userId)
    {
        return Ok(_libraryService.GetUserLibrariesById());
    }
    [HttpGet("GetUserLibraryId")]
    public ActionResult<LibraryDto> GetUserLibraryId(string libraryId)
    {
        return Ok(_libraryService.GetUserLibraryById());
    }
    [HttpGet("GetLibraryUploadsById")]
    public ActionResult<UploadDto[]> GetLibraryUploadsById(string libraryId)
    {
        return Ok(_libraryService.GetLibraryUploadsById());
    }
    [HttpGet("GetLibraryUploadById")]
    public ActionResult<UploadDto> GetLibraryUploadById(string uploadId)
    {
        return Ok(_libraryService.GetLibraryUploadById());
    }
}