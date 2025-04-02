using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ContentLibraryController : ControllerBase
{
    private readonly List<LibraryDto> _mockLibraries;

    private readonly List<UploadDto> _mockUploads = new()
    {
        new UploadDto(
            1,
            1,
            DateTime.Now,
            "image/png",
            1
        )
    };

    [HttpGet("GetUserLibrariesById")]
    public ActionResult<LibraryDto[]> GetUserLibrariesById(string userId)
    {
        return Ok(_mockLibraries);
    }

    [HttpGet("GetUserLibraryId")]
    public ActionResult<LibraryDto> GetUserLibraryId(string libraryId)
    {
        return Ok(_mockLibraries.First());
    }

    [HttpGet("GetLibraryUploadsById")]
    public ActionResult<UploadDto[]> GetLibraryUploadsById(string libraryId)
    {
        return Ok(_mockUploads);
    }

    [HttpGet("GetLibraryUploadById")]
    public ActionResult<UploadDto> GetLibraryUploadById(string uploadId)
    {
        return Ok(_mockUploads.First());
    }
}