using System.Text.Json.Serialization;

namespace API.DTOs;

public class AddUploadRequestDto
{
    [JsonConstructor]
    public AddUploadRequestDto(UploadDto uploadDto, LessonDto lessonDto)
    {
        UploadDto = uploadDto;
        LessonDto = lessonDto;
    }

    public UploadDto UploadDto { get; }
    public LessonDto LessonDto { get; }
}