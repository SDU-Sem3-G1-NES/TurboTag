using API.DTOs;
using API.Services;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController(IUploadService uploadService) : ControllerBase
{
    [HttpPost("GetAllUploads")]
    public ActionResult<IEnumerable<UploadDto>> GetAllUploads([FromBody] UploadFilter? filter)
    {
        return Ok(uploadService.GetAllUploads(filter));
    }
    
    [HttpPost("GetUserUploads")]
    public ActionResult<IEnumerable<UploadDto>> GetUserUploads([FromBody] (UserDto user, UploadFilter filter) parameters)
    {
        return Ok(uploadService.GetUserUploads(parameters.user, parameters.filter));
    }
    
    [HttpPost("GetLibraryUploads")]
    public ActionResult<IEnumerable<UploadDto>> GetLibraryUploads(int libraryId, [FromBody] UploadFilter filter)
    {
        return Ok(uploadService.GetLibraryUploads(libraryId, filter));
    }
    
    [HttpGet("GetUploadById")]
    public ActionResult<UploadDto> GetUploadById(int uploadId)
    {
        return Ok(uploadService.GetUploadById(uploadId));
    }
    
    [HttpPost("AddUpload")]
    public ActionResult<int> AddUpload([FromBody] UploadDto upload)
    {
        uploadService.CreateNewUpload(upload);
        return Ok();
    }
    
    [HttpPut("UpdateUpload")]
    public ActionResult UpdateUpload([FromBody] UploadDto updatedUpload)
    {
        uploadService.UpdateUpload(updatedUpload);
        return Ok();
    }
    
    [HttpDelete("DeleteUploadById")]
    public ActionResult DeleteUploadById(int uploadId)
    {
        uploadService.DeleteUploadById(uploadId);
        return Ok();
    }
}