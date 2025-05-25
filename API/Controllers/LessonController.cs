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
    public ActionResult<int> AddLessonAndTriggerGeneration([FromBody] LessonDto lesson)
    {
        lessonService.AddLesson(lesson);

        var transcription = lessonService.GetTranscriptionByObjectId(lesson.FileMetadata.First().Id);
        if (!string.IsNullOrWhiteSpace(transcription))
        {
            var httpClient = new HttpClient();
            var payload = new
            {
                uploadId = lesson.UploadId,
                text = "Hi everyone, and welcome to your first piano lesson. I’m really glad you’re here! Today, we’re going to learn a few foundational things:\n\n1. How to sit at the piano correctly\n2. How to find middle C\n3. How to play your first five notes using your right hand\n\nLet’s start with posture. Sit on the front half of the bench with your feet flat on the floor. Your elbows should be slightly in front of your body, and your fingers should rest gently on the keys, curved like you're holding a small ball.\n\nNow, let’s find middle C. Look at your keyboard and find the group of two black keys in the center. Middle C is the white key just to the left of that group. That’s your home base.\n\nLet’s place your right-hand thumb (finger 1) on middle C. Then let fingers 2, 3, 4, and 5 naturally rest on the next four white keys: D, E, F, and G.\n\nNow, we’ll play each finger slowly, one at a time:\n- Thumb on C\n- Finger 2 on D\n- Finger 3 on E\n- Finger 4 on F\n- Finger 5 on G\n\nGreat! That’s called a 5-finger scale.\n\nTry playing it forward and backward, nice and slow. C–D–E–F–G… and back: G–F–E–D–C.\n\nListen to how each note sounds. Make sure they’re even and smooth, like stepping stones in a row.\n\nLet’s repeat that a few times together.\n\nNow, here’s your first rhythm exercise: We’ll play each note twice, like this:\nC–C, D–D, E–E, F–F, G–G\nNow backwards: G–G, F–F, E–E, D–D, C–C\n\nVery good!\n\nThat’s it for today. Remember: practice this right-hand 5-finger scale every day for a few minutes. It will build finger strength, accuracy, and confidence.\n\nIn the next lesson, we’ll use the left hand and begin playing simple songs.\n\nSee you next time, and happy playing!"
            };
            httpClient.PostAsJsonAsync("http://localhost:8001/generate-async", payload);
        }

        return Ok(lesson.UploadId);
    }

    [AllowAnonymous]
    [HttpPost("CompleteGeneration")]
    public IActionResult CompleteGeneration([FromBody] LessonCompletionDto result)
    {
        try
        {
            var lesson = lessonService.GetLessonByUploadId(result.UploadId);
            if (lesson == null) return NotFound();

            lesson.LessonDetails.Description = result.Description ?? "";
            
            lesson.LessonDetails.Tags = result.Tags?.Split(',').Select(t => t.Trim()).ToList() ?? [];

            lessonService.UpdateLesson(lesson);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error completing generation: {ex.Message}");
        }
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

public class LessonCompletionDto
{
    public int UploadId { get; set; }
    public string? Tags { get; set; }
    public string? Description { get; set; }
}