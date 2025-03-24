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
        return Ok(mockLibraries);
    }
    [HttpGet("GetUserLibraryId")]
    public ActionResult<LibraryDTO> GetUserLibraryId(string libraryId)
    {
        return Ok(mockLibraries.First());
    }
    [HttpGet("GetLibraryUploadsById")]
    public ActionResult<UploadDTO[]> GetLibraryUploadsById(string libraryId)
    {
        return Ok(mockUploads);
    }
    [HttpGet("GetLibraryUploadById")]
    public ActionResult<UploadDTO> GetLibraryUploadById(string uploadId)
    {
        return Ok(mockUploads.First());
    }
    
    private List<LibraryDTO> mockLibraries = new List<LibraryDTO>
    {
        new LibraryDTO(libraryId: 1, libraryName: "Library 1")
    };
    private List<UploadDTO> mockUploads = new List<UploadDTO>
    {
        new UploadDTO(
            id: 1,
            ownerId: 1,
            libraryId: 1,
            details: new UploadDetailsDTO(id: 1,
                description: "Description 1",
                title: "Title 1", tags:
                new List<string> { "Tag 1", "Tag 2" }),
            fileMetadata: new FileMetadataDTO(id: 1,
                fileType: "txt",
                fileName: "example.txt",
                fileSize: 123.45f,
                duration: 60,
                date: "2023-10-01",
                checkSum: "abc123")
        )
    };
}
