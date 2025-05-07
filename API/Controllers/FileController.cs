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
            if (string.IsNullOrWhiteSpace(uploadChunkDto.UploadId) || 
                !System.Text.RegularExpressions.Regex.IsMatch(uploadChunkDto.UploadId, @"^[a-zA-Z0-9_-]+$"))
            {
                return BadRequest("Invalid UploadId. Only alphanumeric characters, underscores, and hyphens are allowed.");
            }
            
            if (string.IsNullOrWhiteSpace(uploadChunkDto.UploadId) || 
                uploadChunkDto.UploadId.Contains("..") || 
                uploadChunkDto.UploadId.Contains(Path.DirectorySeparatorChar) || 
                uploadChunkDto.UploadId.Contains(Path.AltDirectorySeparatorChar))
            {
                return BadRequest("Invalid UploadId");
            }
            
            var tempPath = Path.Combine(Path.GetTempPath(), "uploads", uploadChunkDto.UploadId);
            Directory.CreateDirectory(tempPath);
            var chunkPath = Path.Combine(tempPath, $"{uploadChunkDto.ChunkNumber}.part");
            await System.IO.File.WriteAllBytesAsync(chunkPath, uploadChunkDto.Chunk);
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
        // Validate UploadId
        if (string.IsNullOrWhiteSpace(finaliseUploadDto.UploadId) || 
            finaliseUploadDto.UploadId.Contains("..") || 
            finaliseUploadDto.UploadId.Contains(Path.DirectorySeparatorChar) || 
            finaliseUploadDto.UploadId.Contains(Path.AltDirectorySeparatorChar))
        {
            return BadRequest("Invalid UploadId");
        }

        // Validate FileName
        if (string.IsNullOrWhiteSpace(finaliseUploadDto.FileName) || 
            finaliseUploadDto.FileName.Contains("..") || 
            finaliseUploadDto.FileName.Contains(Path.DirectorySeparatorChar) || 
            finaliseUploadDto.FileName.Contains(Path.AltDirectorySeparatorChar))
        {
            return BadRequest("Invalid FileName");
        }
        
        if (string.IsNullOrWhiteSpace(finaliseUploadDto.UploadId) || 
            finaliseUploadDto.UploadId.Contains("..") || 
            finaliseUploadDto.UploadId.Contains(Path.DirectorySeparatorChar) || 
            finaliseUploadDto.UploadId.Contains(Path.AltDirectorySeparatorChar))
        {
            return BadRequest("Invalid UploadId");
        }
        
        if (string.IsNullOrWhiteSpace(finaliseUploadDto.FileName) || 
            finaliseUploadDto.FileName.Contains("..") || 
            finaliseUploadDto.FileName.Contains(Path.DirectorySeparatorChar) || 
            finaliseUploadDto.FileName.Contains(Path.AltDirectorySeparatorChar))
        {
            return BadRequest("Invalid FileName");
        }

        var tempDir = Path.Combine(Path.GetTempPath(), "uploads", finaliseUploadDto.UploadId);

        if (!Directory.Exists(tempDir))
        {
            return StatusCode(500, $"Upload directory not found for ID: {finaliseUploadDto.UploadId}");
        }

        var outputPath = Path.Combine(tempDir, finaliseUploadDto.FileName);
        
        int totalChunks = 0;
        while (System.IO.File.Exists(Path.Combine(tempDir, $"{totalChunks}.part")))
        {
            totalChunks++;
        }
        
        if (totalChunks == 0)
        {
            return StatusCode(500, "No chunks found to combine");
        }
        
        try
        {
            using (var outputStream = new FileStream(outputPath, FileMode.Create))
            {
                for (int i = 0; i < totalChunks; i++)
                {
                    var chunkPath = Path.Combine(tempDir, $"{i}.part");
                    
                    if (!System.IO.File.Exists(chunkPath))
                    {
                        return StatusCode(500, $"Chunk {i} missing");
                    }
                    
                    try
                    {
                        using var chunkStream = System.IO.File.OpenRead(chunkPath);
                        await chunkStream.CopyToAsync(outputStream);
                        // Close stream before deleting
                        chunkStream.Close();
                        System.IO.File.Delete(chunkPath);
                    }
                    catch (Exception chunkEx)
                    {
                        return StatusCode(500, $"Error processing chunk {i}: {chunkEx.Message}");
                    }
                }
            }
            
            await using var finalStream = System.IO.File.OpenRead(outputPath);

            var fileId = await fileService.UploadChunkedFile(finalStream, finaliseUploadDto.FileName);
            
            
            try
            {
                System.IO.File.Delete(outputPath);
                Directory.Delete(tempDir, true);
            }
            catch (Exception cleanupEx)
            {
                Console.WriteLine($"Cleanup error: {cleanupEx.Message}");
            }
            
            return Ok(new { fileId });
        }
        catch (Exception ioEx)
        {
            return StatusCode(500, $"IO error while combining chunks: {ioEx.Message}");
        }
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
    public IFormFile? File { get; set; }
}

public class UploadChunkDto
{
    public byte[]? Chunk { get; set; } 
    public string? UploadId { get; set; }
    public int ChunkNumber { get; set; }
}


public class FinaliseUploadDto
{
    public string? UploadId { get; set; }
    public string? FileName { get; set; }
}