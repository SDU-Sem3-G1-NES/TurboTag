using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]

[Route("[controller]")]
public class FileController(IFileService fileService) : ControllerBase
{
    [RequestSizeLimit(long.MaxValue)]
    [HttpPost("UploadFile")]
    public async Task<ActionResult> UploadFile(IFormFile file)
    {
        var fileId = await fileService.UploadFile(file);
        return Ok(fileId);
    }
    [RequestSizeLimit(long.MaxValue)]
    [HttpGet("GetFile")]
    public async Task<ActionResult> GetFile(string id)
    {
        var file = await fileService.GetFile(id);
        if (file == null)
        {
            return NotFound();
        }
        return File(file, "application/octet-stream", id);
    }
    [HttpDelete("DeleteFile")]
    public ActionResult DeleteFile(string id)
    {
        fileService.DeleteFile(id);
        return Ok();
    }
}