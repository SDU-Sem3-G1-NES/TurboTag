using System.Diagnostics;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ContentLibraryController : ControllerBase
{
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
    
    private readonly List<LibraryDto> _mockLibraries = new List<LibraryDto>
    {
        new LibraryDto(id: 1, name: "Library 1")
    };
    private readonly List<UploadDto> _mockUploads = new List<UploadDto>
    {
        new UploadDto(
            id: 1,
            ownerId: 1,
            date: DateTime.Now,
            type: "image/png",
            libraryId: 1
        )
    };
}
