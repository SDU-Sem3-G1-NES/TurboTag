using API.DTOs;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TagController(ITagService tagService) : ControllerBase
{
    [HttpPost("GetAllTags")]
    public ActionResult<IEnumerable<TagDto>> GetAllTags([FromBody] TagFilter? filter)
    {
        return Ok(tagService.GetAllTags(filter));
    }

    [HttpPost("AddTag")]
    public ActionResult AddTag([FromBody] TagDto request)
    {
        try
        {
            var added = tagService.AddTag(request);
            if (!added) return Conflict("A tag with the same name already exists.");

            return Ok(); // Or return Created(...) if you want to return a location or ID later
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpDelete("DeleteTagById")]
    public ActionResult DeleteTagById(TagFilter? filter)
    {
        tagService.DeleteTags(filter);
        return Ok();
    }
}