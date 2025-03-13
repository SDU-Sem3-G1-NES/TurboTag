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
        return Ok(libraries);
    }
    [HttpGet("GetUserLibraryId")]
    public ActionResult<LibraryDTO> GetUserLibraryId(string libraryId)
    {
        return Ok(libraries.First());
    }
    [HttpGet("GetLibraryUploadsById")]
    public ActionResult<UploadDTO[]> GetLibraryUploadsById(string libraryId)
    {
        return Ok(uploads);
    }
    [HttpGet("GetLibraryUploadById")]
    public ActionResult<UploadDTO> GetLibraryUploadById(string uploadId)
    {
        return Ok(uploads.First());
    }
    
    private List<LibraryDTO> libraries = new List<LibraryDTO>
    {
        new LibraryDTO(libraryId: 1, libraryName: "Library 1")
    };
    private List<UploadDTO> uploads = new List<UploadDTO>
    {
        new UploadDTO(
            uploadId: 1,
            ownerId: 1,
            uploadDetails: new UploadDetailsDTO(uploadId: 1,
                uploadDescription: "Description 1",
                uploadTitle: "Title 1", uploadTags:
                new List<string> { "Tag 1", "Tag 2" }),
            fileMetadata: new FileMetadataDTO(uploadId: 1,
                fileType: "txt",
                fileName: "example.txt",
                fileSize: 123.45f,
                duration: 60,
                uploadDate: "2023-10-01",
                checkSum: "abc123")
        )
    };
}
