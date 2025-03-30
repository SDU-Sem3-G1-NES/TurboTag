using System.Diagnostics;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    UploadService _uploadService;
    [HttpPost("StoreUpload")]
    public ActionResult StoreUpload(UploadDto upload)
    {
        try
        {
            _uploadService.StoreUpload(upload);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
           
        }
        
    }
}