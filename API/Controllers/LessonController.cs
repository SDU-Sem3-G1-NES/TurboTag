using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]

[Route("[controller]")]
public class LessonController(ILessonService lessonService) : ControllerBase
{
    [HttpGet("GetAllLessons")]
    public ActionResult GetAllLessons()
    {
        return Ok(lessonService.GetAllLessons());
    }

    [HttpGet("GetLessonById")]
    public ActionResult GetLessonById(string lessonId)
    {
        return Ok(lessonService.GetLessonById(lessonId));
    }

    [HttpGet("GetLessonByUploadId")]
    public ActionResult GetLessonByUploadId(int uploadId)
    {
        return Ok(lessonService.GetLessonByUploadId(uploadId));
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

    [HttpDelete("DeleteLesson")]
    public ActionResult DeleteLesson(int uploadId)
    {
        lessonService.DeleteLesson(uploadId);
        return Ok();
    }
}