using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]

[Route("[controller]")]
public class LessonController(ILessonService lessonService) : ControllerBase
{
    [HttpGet("GetAllLessons")]
    public ActionResult<List<LessonDto>> GetAllLessons()
    {
        return Ok(lessonService.GetAllLessons());
    }
    
    [HttpGet("GetLessonsByTags")]
    public ActionResult<List<LessonDto>> GetLessonsByTags([FromQuery] string[] tags)
    {
        return Ok(lessonService.GetLessonsByTags(tags));
    }
    
    [HttpGet("GetLessonsByTitle")]
    public ActionResult<List<LessonDto>> GetLessonsByTitle(string title)
    {
        return Ok(lessonService.GetLessonsByTitle(title));
    }
    [HttpGet("GetLessonById")]
    public ActionResult<LessonDto> GetLessonById(int lessonId)
    {
        var result = lessonService.GetLessonById(lessonId);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
    
    [HttpGet("GetLessonByObjectId")]
    public ActionResult<LessonDto> GetLessonByObjectId(string objectId)
    {
        var result = lessonService.GetLessonByObjectId(objectId);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpGet("GetLessonByUploadId")]
    public ActionResult<LessonDto> GetLessonByUploadId(int uploadId)
    {
        var result = lessonService.GetLessonByUploadId(uploadId);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost("AddLesson")]
    public ActionResult AddLesson([FromBody] LessonDto lesson)
    {
        lessonService.AddLesson(lesson);
        return Ok();
    }

    [HttpPut("UpdateLesson")]
    public ActionResult UpdateLesson([FromBody] LessonDto lesson)
    {
        lessonService.UpdateLesson(lesson);
        return Ok();
    }

    [HttpDelete("DeleteLessonById")]
    public ActionResult DeleteLessonById(int lessonId)
    {
        lessonService.DeleteLessonById(lessonId);
        return Ok();
    }
    
    [HttpDelete("DeleteLessonByObjectId")]
    public ActionResult DeleteLessonByObjectId(string objectId)
    {
        lessonService.DeleteLessonByObjectId(objectId);
        return Ok();
    }
}