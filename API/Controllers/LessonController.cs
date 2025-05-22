using API.DTOs;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class LessonController(ILessonService lessonService) : ControllerBase
{
    [HttpPost("GetAllLessons")]
    public ActionResult<IEnumerable<LessonDto>> GetAllLessons([FromBody] LessonFilter? filter)
    {
        return Ok(lessonService.GetAllLessons(filter));
    }

    [HttpPost("StarLesson")]
    public ActionResult StarLesson([FromBody] int lessonId, int userId)
    {
        lessonService.StarLesson(lessonId, userId);
        return Ok();
    }

    [HttpPost("UnstarLesson")]
    public ActionResult UnstarLesson([FromBody] int lessonId, int userId)
    {
        lessonService.UnstarLesson(lessonId, userId);
        return Ok();
    }

    [HttpGet("GetLessonsByTags")]
    public ActionResult<IEnumerable<LessonDto>> GetLessonsByTags([FromQuery] string[] tags)
    {
        return Ok(lessonService.GetLessonsByTags(tags));
    }

    [HttpGet("GetLessonsByTitle")]
    public ActionResult<IEnumerable<LessonDto>> GetLessonsByTitle(string title)
    {
        return Ok(lessonService.GetLessonsByTitle(title));
    }

    [HttpGet("GetLessonById")]
    public ActionResult<LessonDto> GetLessonById(int lessonId)
    {
        var result = lessonService.GetLessonById(lessonId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("GetTranscriptionByObjectId")]
    public string GetTranscriptionByObjectId(string objectId)
    {
        var result = lessonService.GetTranscriptionByObjectId(objectId);
        if (result == null) return "";
        return result;
    }

    [HttpGet("GetLessonByObjectId")]
    public ActionResult<LessonDto> GetLessonByObjectId(string objectId)
    {
        var result = lessonService.GetLessonByObjectId(objectId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("GetLessonByUploadId")]
    public ActionResult<LessonDto> GetLessonByUploadId(int uploadId)
    {
        var result = lessonService.GetLessonByUploadId(uploadId);
        if (result == null) return NotFound();
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