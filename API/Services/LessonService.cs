using API.Dtos;
using API.Repositories;

namespace API.Services;

public interface ILessonService : IServiceBase
{
    void AddLesson(LessonDto lesson);
    LessonDto? GetLessonById(string lessonId);
    LessonDto? GetLessonByUploadId(int uploadId);
    List<LessonDto> GetAllLessons();
    void UpdateLesson(LessonDto lesson);
    void DeleteLesson(int uploadId);
}
public class LessonService(ILessonRepository lessonRepository) : ILessonService
{
    public void AddLesson(LessonDto lesson)
    {
        lessonRepository.AddLesson(lesson);
    }
    public LessonDto? GetLessonById(string lessonId)
    {
        return lessonRepository.GetLessonById(lessonId);
    }
    public LessonDto? GetLessonByUploadId(int uploadId)
    {
        return lessonRepository.GetLessonByUploadId(uploadId);
    }
    public List<LessonDto> GetAllLessons()
    {
        return lessonRepository.GetAllLessons();
    }
    public void UpdateLesson(LessonDto lesson)
    {
        lessonRepository.UpdateLesson(lesson);
    }
    public void DeleteLesson(int uploadId)
    {
        lessonRepository.DeleteLesson(uploadId);
    }
}