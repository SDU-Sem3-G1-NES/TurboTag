using System.Diagnostics;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ContentLibraryController : ControllerBase
{
    [HttpGet("GetUserLibrariesById")]
    public ActionResult<LibraryDTO[]> GetUserLibrariesById(string userId)
    {
        return Ok(_mockLibraries);
    }
    [HttpGet("GetUserLibraryId")]
    public ActionResult<LibraryDTO> GetUserLibraryId(string libraryId)
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
    
    private readonly List<LibraryDTO> _mockLibraries = new List<LibraryDTO>
    {
        new LibraryDTO(libraryId: 1, libraryName: "Library 1")
    };
    private readonly List<UploadDto> _mockUploads = new List<UploadDto>
    {
        new UploadDto(
            id: 1,
            ownerId: 1,
            libraryId: 1,
            details: new UploadDetailsDto(id: 1,
                description: "Description 1",
                title: "Title 1", 
                tags:
                    new List<string> { "Tag 1", "Tag 2" }),
            fileMetadata: new FileMetadataDto(id: 1,
                fileType: "txt",
                fileName: "example.txt",
                fileSize: 123.45f,
                duration: 60,
                date: DateTime.Now, 
                checkSum: "abc123")
        )
    };
}
