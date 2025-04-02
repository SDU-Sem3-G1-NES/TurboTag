using API.Dtos;
using API.Repositories;

namespace API.Services;

public interface ILessonService : IServiceBase
{
    void AddLesson(LessonDto lesson);
    public List<LessonDto>? GetLessonsByTags(string[] tags);
    public List<LessonDto>? GetLessonsByTitle(string title);
    LessonDto? GetLessonById(string lessonId);
    LessonDto? GetLessonByObjectId(string objectId);
    LessonDto? GetLessonByUploadId(int uploadId);
    List<LessonDto>? GetAllLessons();
    void UpdateLesson(LessonDto lesson);
    void DeleteLessonById(int uploadId);
    void DeleteLessonByObjectId(string objectId);
}
public class LessonService(ILessonRepository lessonRepository) : ILessonService
{
    public void AddLesson(LessonDto lesson)
    {
        lessonRepository.AddLesson(lesson);
    }
    public List<LessonDto>? GetLessonsByTags(string[] tags)
    {
        return lessonRepository.GetLessonsByTags(tags);
    }

    public List<LessonDto>? GetLessonsByTitle(string title)
    {
        return lessonRepository.GetLessonsByTitle(title);
    }
    public LessonDto? GetLessonById(string lessonId)
    {
        return lessonRepository.GetLessonById(lessonId);
    }
    public LessonDto? GetLessonByUploadId(int uploadId)
    {
        return lessonRepository.GetLessonByUploadId(uploadId);
    }
    public LessonDto? GetLessonByObjectId(string objectId)
    {
        return lessonRepository.GetLessonByObjectId(objectId);
    }
    public List<LessonDto>? GetAllLessons()
    {
        return lessonRepository.GetAllLessons();
    }
    public void UpdateLesson(LessonDto lesson)
    {
        lessonRepository.UpdateLesson(lesson);
    }
    public void DeleteLessonById(int lessonId)
    {
        lessonRepository.DeleteLessonById(lessonId);
    }
    public void DeleteLessonByObjectId(string objectId)
    {
        lessonRepository.DeleteLessonByObjectId(objectId);
    }
}