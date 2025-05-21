using System.Text.Json;
using API.DataAccess;
using API.DTOs;
using MongoDB.Bson;

namespace API.Repositories;

public interface ILessonRepository : IRepositoryBase
{
    void AddLesson(LessonDto lesson);
    List<LessonDto> GetAllLessons(LessonFilter? filter);
    public List<LessonDto> GetLessonsByTags(string[] tags);
    public List<LessonDto> GetLessonsByTitle(string title);
    LessonDto? GetLessonByObjectId(string objectId);
    LessonDto? GetLessonById(int lessonId);
    LessonDto? GetLessonByUploadId(int uploadId);
    void UpdateLesson(LessonDto lesson);
    void DeleteLessonById(int lessonId);
    void DeleteLessonByObjectId(string objectId);
}

public class LessonRepository(IMongoDataAccess database) : ILessonRepository
{
    public void AddLesson(LessonDto lesson)
    {
        lesson.GenerateMongoId();
        database.Insert("lesson", lesson.ToBsonDocument());
    }

    public List<LessonDto> GetAllLessons(LessonFilter? filter = null)
    {
        var query = new List<string>();

        if (filter != null)
        {
            if (filter.UploadIds is { Count: > 0 })
                query.Add($"{{\"upload_id\": {{$all: [{string.Join(",", filter.UploadIds)}]}}}}");

            if (!string.IsNullOrEmpty(filter.Title))
            {
                var escapedTitle = JsonSerializer.Serialize(filter.Title).Trim('"');
                query.Add($"{{\"lesson_details.title\": {{$regex: \"{escapedTitle}\", $options: \"i\"}}}}");
            }

            if (filter.OwnerId != null)
                query.Add($"{{\"owner_id\": {filter.OwnerId}}}");

            if (filter.LessonId != null)
                query.Add($"{{\"lesson_details._id\": {filter.LessonId}}}");

            if (filter.Tags is { Count: > 0 })
            {
                var escapedTags = filter.Tags.Select(tag => JsonSerializer.Serialize(tag));
                query.Add($"{{\"lesson_details.tags\": {{$all: [{string.Join(",", escapedTags)}]}}}}");
            }

            if (filter is { IsStarred: true, StarredLessons.Length: > 0 })
            {
                var uploadIdList = string.Join(",", filter.StarredLessons);
                query.Add($"{{\"upload_id\": {{ \"$in\": [{uploadIdList}] }} }}");
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                var escapedSearchText = JsonSerializer.Serialize(filter.SearchText).Trim('"');
                query.Add($@"{{
            ""$or"": [
                {{""lesson_details.title"": {{""$regex"": ""{escapedSearchText}"", ""$options"": ""i""}} }},
                {{""lesson_details.description"": {{""$regex"": ""{escapedSearchText}"", ""$options"": ""i""}} }}
            ]
        }}");
            }
        }


        var filterString = query.Count > 1
            ? $"{{ \"$and\": [{string.Join(",", query)}] }}"
            : query.FirstOrDefault() ?? "{}";

        if (filter is { PageNumber: not null, PageSize: not null })
        {
            var skip = (filter.PageNumber.Value - 1) * filter.PageSize.Value;
            return database.Find<LessonDto>("lesson", filterString, skip, filter.PageSize);
        }

        return database.Find<LessonDto>("lesson", filterString);
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
        database.Delete("lesson", $"{{\"lesson_details._id\": {lessonId}}}");
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

public class LessonFilter : PaginationFilter
{
    public LessonFilter(List<int>? uploadIds,
        string? title,
        int? lessonId,
        int? ownerId,
        int? userId,
        List<string>? tags,
        int? pageNumber,
        int? pageSize,
        string? searchText,
        bool? isStarred,
        int[]? starredLessons) : base(pageNumber, pageSize)
    {
        UploadIds = uploadIds;
        Title = title;
        OwnerId = ownerId;
        UserId = userId;
        Tags = tags;
        LessonId = lessonId;
        PageSize = pageSize;
        PageNumber = pageNumber;
        SearchText = searchText;
        IsStarred = isStarred;
        StarredLessons = starredLessons;
    }

    public LessonFilter()
    {
    }

    public string? Title { get; set; }
    public List<string>? Tags { get; set; }
    public int? OwnerId { get; set; }
    public int? UserId { get; set; }
    public List<int>? UploadIds { get; set; }
    public int? LessonId { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
    public string? SearchText { get; set; }
    public bool? IsStarred { get; set; }
    public int[]? StarredLessons { get; set; }
}