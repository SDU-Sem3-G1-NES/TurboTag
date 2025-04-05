using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface ILessonService : IServiceBase
{
    void AddLesson(LessonDto lesson);
    List<LessonDto> GetAllLessons();
    public List<LessonDto> GetLessonsByTags(string[] tags);
    public List<LessonDto> GetLessonsByTitle(string title);
    LessonDto? GetLessonById(int lessonId);
    LessonDto? GetLessonByObjectId(string objectId);
    LessonDto? GetLessonByUploadId(int uploadId);
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
    
    public List<LessonDto> GetAllLessons()
    {
        return lessonRepository.GetAllLessons();
    }
    
    public List<LessonDto> GetLessonsByTags(string[] tags)
    {
        return lessonRepository.GetLessonsByTags(tags);
    }

    public List<LessonDto> GetLessonsByTitle(string title)
    {
        return lessonRepository.GetLessonsByTitle(title);
    }
    public LessonDto? GetLessonById(int lessonId)
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