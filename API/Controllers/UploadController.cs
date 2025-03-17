using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController(IUploadService _uploadService) : ControllerBase
{
    [HttpPost("StoreUpload")]
    public ActionResult<string> StoreUpload()
    {

        return Ok(_uploadService.StoreUpload());
    }
}