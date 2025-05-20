using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface ILessonService : IServiceBase
{
    void AddLesson(LessonDto lesson);
    List<LessonDto> GetAllLessons(LessonFilter? filter);
    public List<LessonDto> GetLessonsByTags(string[] tags);
    public List<LessonDto> GetLessonsByTitle(string title);
    LessonDto? GetLessonById(int lessonId);
    LessonDto? GetLessonByObjectId(string objectId);
    LessonDto? GetLessonByUploadId(int uploadId);
    void UpdateLesson(LessonDto lesson);
    void DeleteLessonById(int uploadId);
    void DeleteLessonByObjectId(string objectId);
}

public class LessonService(ILessonRepository lessonRepository, IUserRepository userRepository) : ILessonService
{
    public void AddLesson(LessonDto lesson)
    {
        lessonRepository.AddLesson(lesson);
    }

    public List<LessonDto> GetAllLessons(LessonFilter? filter)
    {
        return AddOwners(lessonRepository.GetAllLessons(filter));
    }

    public List<LessonDto> GetLessonsByTags(string[] tags)
    {
        return AddOwners(lessonRepository.GetLessonsByTags(tags));
    }

    public List<LessonDto> GetLessonsByTitle(string title)
    {
        return AddOwners(lessonRepository.GetLessonsByTitle(title));
    }

    public LessonDto? GetLessonById(int lessonId)
    {
        var lesson = lessonRepository.GetLessonById(lessonId);
        if (lesson is null) return null;

        return AddOwners([lesson]).FirstOrDefault();
    }

    public LessonDto? GetLessonByUploadId(int uploadId)
    {
        var lesson = lessonRepository.GetLessonByUploadId(uploadId);
        if (lesson is null) return null;

        return AddOwners([lesson]).FirstOrDefault();
    }

    public LessonDto? GetLessonByObjectId(string objectId)
    {
        var lesson = lessonRepository.GetLessonByObjectId(objectId);
        if (lesson is null) return null;

        return AddOwners([lesson]).FirstOrDefault();
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

    private List<LessonDto> AddOwners(List<LessonDto> lessons)
    {
        var ownerIds = lessons
            .Select(l => l.OwnerId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
        var ownerNames = userRepository.GetAllUsers(new UserFilter { UserIds = ownerIds }).ToArray();

        foreach (var lesson in lessons)
            if (lesson.OwnerId.HasValue)
            {
                var owner = ownerNames.FirstOrDefault(u => u.Id == lesson.OwnerId.Value);
                lesson.OwnerName = owner?.Name;
            }

        return lessons;
    }
}