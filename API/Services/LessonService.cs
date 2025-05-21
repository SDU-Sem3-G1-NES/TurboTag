using Api.Controllers;
using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface ILessonService : IServiceBase
{
    void AddLesson(LessonDto lesson);
    IEnumerable<LessonDto> GetAllLessons(LessonFilter? filter);
    public List<LessonDto> GetLessonsByTags(string[] tags);
    public List<LessonDto> GetLessonsByTitle(string title);
    LessonDto? GetLessonById(int lessonId);
    LessonDto? GetLessonByObjectId(string objectId);
    LessonDto? GetLessonByUploadId(int uploadId);
    void UpdateLesson(LessonDto lesson);
    void DeleteLessonById(int uploadId);
    void DeleteLessonByObjectId(string objectId);
    void StarLesson(int lessonId, int userId);
    void UnstarLesson(int lessonId, int userId);
    public string? GetTranscriptionByObjectId(string objectId);
    Dictionary<string, int[]> TagOptions(BaseOptionsFilter filter);
    Dictionary<int, string> UploaderOptions(BaseOptionsFilter filter);
}

internal class LessonService(
    ILessonRepository lessonRepository,
    IUserRepository userRepository,
    IUploadRepository uploadRepository) : ILessonService
{
    public void AddLesson(LessonDto lesson)
    {
        lessonRepository.AddLesson(lesson);
    }
    
    public string? GetTranscriptionByObjectId(string objectId)
    {
        var transcription = lessonRepository.GetTranscriptionByObjectId(objectId);
        if (transcription is null) return null;

        return transcription;
    }

    public List<LessonDto> GetLessonsByTags(string[] tags)
    {
        return AddOwnersAndStars(lessonRepository.GetLessonsByTags(tags)).ToList();
    }

    public List<LessonDto> GetLessonsByTitle(string title)
    {
        return AddOwnersAndStars(lessonRepository.GetLessonsByTitle(title)).ToList();
    }

    public LessonDto? GetLessonById(int lessonId)
    {
        var lesson = lessonRepository.GetLessonById(lessonId);
        if (lesson is null) return null;

        return AddOwnersAndStars([lesson]).FirstOrDefault();
    }

    public LessonDto? GetLessonByUploadId(int uploadId)
    {
        var lesson = lessonRepository.GetLessonByUploadId(uploadId);
        if (lesson is null) return null;

        return AddOwnersAndStars([lesson]).FirstOrDefault();
    }

    public LessonDto? GetLessonByObjectId(string objectId)
    {
        var lesson = lessonRepository.GetLessonByObjectId(objectId);
        if (lesson is null) return null;

        return AddOwnersAndStars([lesson]).FirstOrDefault();
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

    public void StarLesson(int lessonId, int userId)
    {
        uploadRepository.StarUpload(lessonId, userId);
    }

    public void UnstarLesson(int lessonId, int userId)
    {
        uploadRepository.UnstarUpload(lessonId, userId);
    }

    public Dictionary<string, int[]> TagOptions(BaseOptionsFilter filter)
    {
        return lessonRepository.TagOptions(filter);
    }

    public Dictionary<int, string> UploaderOptions(BaseOptionsFilter filter)
    {
        return lessonRepository.UploaderOptions(filter);
    }

    public IEnumerable<LessonDto> GetAllLessons(LessonFilter? filter)
    {
        return AddOwnersAndStars(lessonRepository.GetAllLessons(AddStarredLessonsToFilter(filter)), filter);
    }

    private IEnumerable<LessonDto> AddOwnersAndStars(IEnumerable<LessonDto> lessons, LessonFilter? filter = null)
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

        if (filter?.UserId == null) return lessons;

        var starredLessons = uploadRepository.GetStarredUploads(filter.UserId.Value);
        if ((starredLessons == null || starredLessons.Length == 0) &&
            (filter.IsStarred == null || (bool)!filter.IsStarred)) return lessons;

        foreach (var lesson in lessons)
            if (lesson.UploadId.HasValue)
                if (starredLessons != null)
                    lesson.IsStarred = starredLessons.Any(u => u == lesson.UploadId.Value);

        if (filter.IsStarred == null || (bool)!filter.IsStarred) return lessons;
        lessons = lessons.Where(l => l.IsStarred).ToList();

        return lessons;
    }

    private LessonFilter? AddStarredLessonsToFilter(LessonFilter? filter)
    {
        if (filter?.UserId == null || filter.IsStarred == null || (bool)!filter.IsStarred) return filter;
        filter.StarredLessons = uploadRepository.GetStarredUploads(filter.UserId.Value);
        return filter;
    }
}