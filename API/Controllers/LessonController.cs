using API.DTOs;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class LessonController(
    ILessonService lessonService,
    IFFmpegService ffmpegService,
    IAudioTranscriptionService audioTranscriptionService) : ControllerBase
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

    [HttpPost("AddLessonAndTriggerGeneration")]
    public ActionResult<int> AddLessonAndTriggerGeneration([FromBody] LessonUploadRequest request)
    {
        var lesson = request.Lesson;
        lessonService.AddLesson(lesson);

        _ = Task.Run(async () =>
        {
            try
            {
                var audioPaths = await ffmpegService.GetVideoAudio(request.OutputPath, request.FileId, false);
                var transcription = await audioTranscriptionService.AudioTranscriptionAsync(audioPaths);

                if (!string.IsNullOrWhiteSpace(transcription))
                {
                    using var httpClient = new HttpClient
                    {
                        Timeout = TimeSpan.FromMinutes(5)
                    };

                    var payload = new
                    {
                        uploadId = lesson.UploadId,
                        text = transcription
                    };

                    await httpClient.PostAsJsonAsync("http://localhost:8001/generate-async", payload);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Background generation failed: {e.Message}");
            }
        });

        return Ok(lesson.UploadId);
    }

    [AllowAnonymous]
    [HttpPost("CompleteGeneration")]
    public IActionResult CompleteGeneration([FromBody] LessonCompletionDto result)
    {
        try
        {
            var lesson = lessonService.GetLessonByUploadId(result.UploadId);
            if (lesson is { LessonDetails: null } or null) return NotFound();

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

public class LessonUploadRequest(LessonDto lesson, string fileId, string outputPath)
{
    public LessonDto Lesson { get; set; } = lesson;
    public string FileId { get; set; } = fileId;
    public string OutputPath { get; set; } = outputPath;
}