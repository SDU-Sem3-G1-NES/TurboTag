using API.DataAccess;
using API.Dtos;
using MongoDB.Bson;

namespace API.Repositories;

public interface ILessonRepository : IRepositoryBase
{
    void AddLesson(LessonDto lesson);
    List<LessonDto> GetAllLessons();
    public List<LessonDto> GetLessonsByTags(string[] tags);
    public List<LessonDto> GetLessonsByTitle(string title);
    LessonDto? GetLessonByObjectId(string objectId);
    LessonDto? GetLessonById(int lessonId);
    LessonDto? GetLessonByUploadId(int uploadId);
    void UpdateLesson(LessonDto lesson);
    void DeleteLessonById(int lessonId);
    void DeleteLessonByObjectId(string objectId);
}

public class LessonRepository(IDocumentDataAccess database) : ILessonRepository
{
    public void AddLesson(LessonDto lesson)
    {
        lesson.GenerateMongoId();
        database.Insert("lesson", lesson.ToBsonDocument());
    }

    public List<LessonDto> GetAllLessons()
    {
        return database.Find<LessonDto>("lesson", "{}");
    }
    
    public List<LessonDto> GetLessonsByTags(string[] tags)
    {
        var filter = $"{{\"lesson_details.tags\": {{$all: [{string.Join(",", tags.Select(tag => $"\"{tag}\""))}]}}}}";
        return database.Find<LessonDto>("lesson", filter);
    }
    
    public List<LessonDto> GetLessonsByTitle(string title)
    {
        var filter = $"{{\"lesson_details.title\": {{$regex: \"{title}\", $options: \"i\"}}}}";
        return database.Find<LessonDto>("lesson", filter);
    }
    
    public LessonDto? GetLessonById(int lessonId)
    {
        return database.Find<LessonDto>(
                "lesson",
                $"{{\"lesson_details._id\": {lessonId}}}")
            .FirstOrDefault();
    }
    
    public LessonDto? GetLessonByObjectId(string objectId)
    {
        if (!ObjectId.TryParse(objectId, out _))
        {
            Console.WriteLine("Invalid ObjectId format provided while fetching a lesson.");
            return null;
        }
        return database.Find<LessonDto>(
                "lesson",
                $"{{\"_id\": ObjectId(\"{objectId}\")}}")
            .FirstOrDefault();
    }
    
    public LessonDto? GetLessonByUploadId(int uploadId)
    {
        return database.Find<LessonDto>(
                "lesson",
                $"{{upload_id: {uploadId}}}")
            .FirstOrDefault();
    }

    public void UpdateLesson(LessonDto lesson)
    {
        database.Replace("lesson", $"{{\"_id\": ObjectId(\"{lesson.MongoId}\")}}", lesson);
    }

    public void DeleteLessonById(int lessonId)
    {
        database.Delete( "lesson", $"{{\"lesson_details._id\": {lessonId}}}");
    }
    public void DeleteLessonByObjectId(string objectId)
    {
        if (!ObjectId.TryParse(objectId, out _))
        {
            Console.WriteLine("Invalid ObjectId format provided while deleting a lesson.");
            return;
        }
        database.Delete("lesson", $"{{\"_id\": ObjectId(\"{objectId}\")}}");
    }
}