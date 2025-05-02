using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]

[Route("[controller]")]
public class FileController(IFileService fileService) : ControllerBase
{

    #region UploadFile
    
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [HttpPost("UploadFile")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> UploadFile([FromForm] FormFileDto formFileDtofile)
    {
        var fileId = await fileService.UploadFile(formFileDtofile.File);
        return Ok(fileId);
    }
    #endregion
    #region GetFileById
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
    #endregion
    #region DeleteFile
    [HttpDelete("DeleteFile")]
    public ActionResult DeleteFile(string id)
    {
        fileService.DeleteFile(id);
        return Ok();
    }
    #endregion
    #region UploadChunk
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [HttpPost("UploadChunk")]
    public async Task<IActionResult> UploadChunk([FromBody] UploadChunkDto uploadChunkDto)
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "uploads", uploadChunkDto.UploadId);
            Directory.CreateDirectory(tempPath);

            var chunkPath = Path.Combine(tempPath, $"{uploadChunkDto.ChunkNumber}.part");
            using var stream = new FileStream(chunkPath, FileMode.Create);
            await uploadChunkDto.Chunk.CopyToAsync(stream);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Chunk upload failed: {ex.Message}");
        }
    }
    
    #endregion
    #region FinalizeUpload
    [HttpPost("FinalizeUpload")]
    public async Task<IActionResult> FinalizeUpload(FinaliseUploadDto finaliseUploadDto)
    {
        try
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "uploads", finaliseUploadDto.UploadId);
            var outputPath = Path.Combine(tempDir, finaliseUploadDto.FileName);
            
            using (var outputStream = new FileStream(outputPath, FileMode.Create))
            {
                for (int i = 0; ; i++)
                {
                    var chunkPath = Path.Combine(tempDir, $"{i}.part");
                    if (!System.IO.File.Exists(chunkPath)) break;
                    
                    using var chunkStream = System.IO.File.OpenRead(chunkPath);
                    await chunkStream.CopyToAsync(outputStream);
                    System.IO.File.Delete(chunkPath);
                }
            }

            using var finalStream = System.IO.File.OpenRead(outputPath);
            var fileId = await fileService.UploadFile(new FormFile(
                finalStream, 
                0, 
                finalStream.Length, 
                finaliseUploadDto.FileName, 
                finaliseUploadDto.FileName
            ));

            Directory.Delete(tempDir, true);
            return Ok(new { fileId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Finalization failed: {ex.Message}");
        }
    }
    #endregion
    
}

public class FormFileDto
{
    public IFormFile File { get; set; }
}

public class UploadChunkDto
{
    public IFormFile Chunk { get; set; }
    public string UploadId { get; set; }
    public int ChunkNumber { get; set; }
}

public class FinaliseUploadDto
{
    public string UploadId { get; set; }
    public string FileName { get; set; }
}