using System.Diagnostics;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    [HttpPost("StoreUpload")]
    public ActionResult StoreUpload()
    {
        return Ok();
    }
}