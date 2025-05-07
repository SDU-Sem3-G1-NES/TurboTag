using API.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]

[Route("[controller]")]
public class FileController(IFileService fileService, IFFmpegService ffmpegService, IMediaProcessingService mediaProcessingService) : ControllerBase
{
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [HttpPost("UploadFile")]
    public async Task<ActionResult<string>> UploadFile(IFormFile file)
    {
        var fileId = await fileService.UploadFile(file);
        
        if (fileId == null)
        {
            return StatusCode(500);
        }
        
        var filePath = await ffmpegService.SaveFileToTemp(file, fileId);
        BackgroundJob.Enqueue(() => mediaProcessingService.ProcessMediaAsync(ffmpegService.GetVideoAudioAndSnapshots(filePath, fileId, true).Result));

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