using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController(IUploadService uploadService) : ControllerBase
{
    [HttpPost("StoreUpload")]
    public ActionResult<string> StoreUpload([FromBody] UploadDto uploadDto)
    {

        return Ok(uploadService.StoreUpload());
    }
}