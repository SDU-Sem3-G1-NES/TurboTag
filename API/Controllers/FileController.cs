using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]

[Route("[controller]")]
public class FileController(IFileService fileService) : ControllerBase
{
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [HttpPost("UploadFile")]
    public async Task<ActionResult<string>> UploadFile(IFormFile file)
    {
        var fileId = await fileService.UploadFile(file);
        return Ok(fileId);
    }
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [HttpGet("GetFileById")]
    public async Task<ActionResult<FileStream>> GetFileById(string id)
    {
        var fileStream = await fileService.GetFileById(id);
        if (fileStream == null)
        {
            return NotFound();
        }
        return File(fileStream, "application/octet-stream", id);
    }
    [HttpDelete("DeleteFile")]
    public ActionResult DeleteFile(string id)
    {
        fileService.DeleteFile(id);
        return Ok();
    }
}