using API.DataAccess;
using API.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Sprache;

namespace API.Repositories;

public interface ILessonRepository : IRepositoryBase
{
    void AddLesson(LessonDto lesson);
    LessonDto? GetLessonById(string lessonId);
    LessonDto? GetLessonByUploadId(int uploadId);
    List<LessonDto> GetAllLessons();
    void UpdateLesson(LessonDto lesson);
    void DeleteLesson(int uploadId);
}

public class LessonRepository(IMongoDb mongoDb) : ILessonRepository
{
    public void AddLesson(LessonDto lesson)
    {
        mongoDb.Insert("lesson", lesson.ToBsonDocument());
    }

    public LessonDto? GetLessonById(string lessonId)
    {
        return mongoDb.Find<LessonDto>(
                "lesson",
                $"{{_id: {{$oid:\"{lessonId}\"}}}}")
            .FirstOrDefault();
    }

    public LessonDto? GetLessonByUploadId(int uploadId)
    {
        return mongoDb.Find<LessonDto>(
                "lesson",
                $"{{upload_id: {uploadId}}}")
            .FirstOrDefault();
    }

    public List<LessonDto> GetAllLessons()
    {
        return mongoDb.Find<LessonDto>("lesson", "{}");
    }

    public void UpdateLesson(LessonDto lesson)
    {
        mongoDb.Replace<LessonDto>("lesson", $"{{upload_id: {lesson.UploadId}}}", lesson); // remake to support multiple uploads
    }

    public void DeleteLesson(int uploadId)
    {
        mongoDb.Delete( "lesson", $"{{upload_id: {uploadId}}}");
    }
}