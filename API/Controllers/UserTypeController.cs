using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserTypeController(IUserTypeService userTypeService) : ControllerBase
{
    [HttpGet("GetAllUserTypes")]
    public ActionResult<IEnumerable<UserTypeDto>> GetAllUserTypes()
    {
        return Ok(userTypeService.GetAllUserTypes());
    }

    [HttpGet("GetUserTypeById")]
    public ActionResult<UserTypeDto> GetUserTypeById(int id)
    {
        return Ok(userTypeService.GetUserTypeById(id));
    }

    [HttpPost("AddUserType")]
    public ActionResult AddUserType([FromBody] UserTypeDto userType)
    {
        userTypeService.AddUserType(userType);
        return Ok();
    }
}