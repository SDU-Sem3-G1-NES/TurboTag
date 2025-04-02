using API.DataAccess;
using API.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Sprache;

namespace API.Repositories;

public interface ILessonRepository : IRepositoryBase
{
    void AddLesson(LessonDto lesson);
    List<LessonDto>? GetAllLessons();
    public List<LessonDto>? GetLessonsByTags(string[] tags);
    public List<LessonDto>? GetLessonsByTitle(string title);
    LessonDto? GetLessonByObjectId(string objectId);
    LessonDto? GetLessonById(string lessonId);
    LessonDto? GetLessonByUploadId(int uploadId);
    void UpdateLesson(LessonDto lesson);
    void DeleteLessonById(int lessonId);
    void DeleteLessonByObjectId(string objectId);
}

public class LessonRepository(IMongoDb mongoDb) : ILessonRepository
{
    public void AddLesson(LessonDto lesson)
    {
        lesson.GenerateMongoId();
        mongoDb.Insert("lesson", lesson.ToBsonDocument());
    }

    public List<LessonDto>? GetAllLessons()
    {
        return mongoDb.Find<LessonDto>("lesson", "{}");
    }
    
    public List<LessonDto>? GetLessonsByTags(string[] tags)
    {
        var filter = $"{{\"lesson_details.tags\": {{$all: [{string.Join(",", tags.Select(tag => $"\"{tag}\""))}]}}}}";
        return mongoDb.Find<LessonDto>("lesson", filter);
    }
    
    public List<LessonDto>? GetLessonsByTitle(string title)
    {
        var filter = $"{{\"lesson_details.title\": {{$regex: \"{title}\", $options: \"i\"}}}}";
        return mongoDb.Find<LessonDto>("lesson", filter);
    }
    
    public LessonDto? GetLessonById(string lessonId)
    {
        return mongoDb.Find<LessonDto>(
                "lesson",
                $"{{\"lesson_details._id\": {lessonId}}}")
            .FirstOrDefault();
    }
    
    public LessonDto? GetLessonByObjectId(string objectId)
    {
        return mongoDb.Find<LessonDto>(
                "lesson",
                $"{{\"_id\": ObjectId(\"{objectId}\")}}")
            .FirstOrDefault();
    }
    
    public LessonDto? GetLessonByUploadId(int uploadId)
    {
        return mongoDb.Find<LessonDto>(
                "lesson",
                $"{{upload_id: {uploadId}}}")
            .FirstOrDefault();
    }

    public void UpdateLesson(LessonDto lesson)
    {
        mongoDb.Replace<LessonDto>("lesson", $"{{\"_id\": ObjectId(\"{lesson.MongoId}\")}}", lesson);
    }

    public void DeleteLessonById(int lessonId)
    {
        mongoDb.Delete( "lesson", $"{{\"lesson_details._id\": {lessonId}}}");
    }
    public void DeleteLessonByObjectId(string objectId)
    {
        mongoDb.Delete("lesson", $"{{\"_id\": ObjectId(\"{objectId}\")}}");
    }
}