using System.Text.Json;
using Api.Controllers;
using API.DataAccess;
using API.DTOs;
using MongoDB.Bson;

namespace API.Repositories;

public interface ILessonRepository : IRepositoryBase
{
    void AddLesson(LessonDto lesson);
    IEnumerable<LessonDto> GetAllLessons(LessonFilter? filter);
    public List<LessonDto> GetLessonsByTags(string[] tags);
    public List<LessonDto> GetLessonsByTitle(string title);
    LessonDto? GetLessonByObjectId(string objectId);
    LessonDto? GetLessonById(int lessonId);
    LessonDto? GetLessonByUploadId(int uploadId);
    void UpdateLesson(LessonDto lesson);
    void DeleteLessonById(int lessonId);
    void DeleteLessonByObjectId(string objectId);

    Dictionary<string, int[]> TagOptions(BaseOptionsFilter filter);
    Dictionary<int, string> UploaderOptions(BaseOptionsFilter filter);
}

public class LessonRepository(IMongoDataAccess database) : ILessonRepository
{
    public void AddLesson(LessonDto lesson)
    {
        lesson.GenerateMongoId();
        database.Insert("lesson", lesson.ToBsonDocument());
    }

    public IEnumerable<LessonDto> GetAllLessons(LessonFilter? filter = null)
    {
        var queryFragments = new List<string>();

        if (filter != null)
        {
            // Filter by upload IDs
            if (filter.UploadIds is { Count: > 0 })
            {
                var uploads = string.Join(",", filter.UploadIds);
                queryFragments.Add($"{{\"upload_id\": {{ \"$all\": [{uploads}] }} }}");
            }

            // Filter by title (regex search, case-insensitive)
            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                var escapedTitle = JsonSerializer.Serialize(filter.Title).Trim('"');
                queryFragments.Add(
                    $"{{\"lesson_details.title\": {{ \"$regex\": \"{escapedTitle}\", \"$options\": \"i\" }} }}");
            }

            // Filter by owner IDs (array or single)
            if (filter.OwnerIdInts is { Length: > 0 })
            {
                var owners = string.Join(",", filter.OwnerIdInts);
                queryFragments.Add($"{{\"owner_id\": {{ \"$in\": [{owners}] }} }}");
            }
            else if (filter.OwnerId != null)
            {
                queryFragments.Add($"{{\"owner_id\": {filter.OwnerId} }}");
            }

            // Filter by single lesson ID
            if (filter.LessonId != null) queryFragments.Add($"{{\"lesson_details._id\": {filter.LessonId} }}");

            // Filter by tags (must match all tags)
            if (filter.Tags is { Count: > 0 })
            {
                var tags = filter.Tags.Select(tag => JsonSerializer.Serialize(tag));
                queryFragments.Add($"{{\"lesson_details.tags\": {{ \"$all\": [{string.Join(",", tags)}] }} }}");
            }

            // Filter by starred lessons
            if (filter.IsStarred == true && filter.StarredLessons?.Length > 0)
            {
                var starred = string.Join(",", filter.StarredLessons);
                queryFragments.Add($"{{\"upload_id\": {{ \"$in\": [{starred}] }} }}");
            }

            // Full-text search
            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                var escapedSearch = JsonSerializer.Serialize(filter.SearchText).Trim('"');
                queryFragments.Add($@"{{
                ""$or"": [
                    {{ ""lesson_details.title"": {{ ""$regex"": ""{escapedSearch}"", ""$options"": ""i"" }} }},
                    {{ ""lesson_details.description"": {{ ""$regex"": ""{escapedSearch}"", ""$options"": ""i"" }} }}
                ]
            }}");
            }
        }

        var filterString = queryFragments.Count switch
        {
            > 1 => $"{{ \"$and\": [{string.Join(",", queryFragments)}] }}",
            1 => queryFragments[0],
            _ => "{}"
        };

        // If paginated → return PagedResult<T>
        if (filter?.PageNumber is not null and > 0 && filter.PageSize is not null and > 0)
        {
            var skip = (filter.PageNumber.Value - 1) * filter.PageSize.Value;
            var pageSize = filter.PageSize.Value;

            var items = database.Find<LessonDto>("lesson", filterString, skip, pageSize);
            var totalCount = database.Count("lesson", filterString);

            return new PagedResult<LessonDto>
            {
                Items = items,
                CurrentPage = filter.PageNumber.Value,
                PageSize = pageSize,
                TotalCount = (int)totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        // Unpaged → return simple list
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

    public Dictionary<string, int[]> TagOptions(BaseOptionsFilter filter)
    {
        var pageNumber = filter.PageNumber ?? 1;
        var pageSize = filter.PageSize ?? int.MaxValue;

        var pipeline = new List<BsonDocument>();

        if (!string.IsNullOrEmpty(filter.SearchText))
        {
            var regex = new BsonRegularExpression(filter.SearchText, "i");
            pipeline.Add(new BsonDocument("$match",
                new BsonDocument("lesson_details.tags", new BsonDocument("$regex", regex))));
        }

        pipeline.Add(new BsonDocument("$unwind", "$lesson_details.tags"));

        if (!string.IsNullOrEmpty(filter.SearchText))
        {
            var regex = new BsonRegularExpression(filter.SearchText, "i");
            pipeline.Add(new BsonDocument("$match",
                new BsonDocument("lesson_details.tags", new BsonDocument("$regex", regex))));
        }

        pipeline.Add(new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$lesson_details.tags" },
            { "uploadIds", new BsonDocument("$addToSet", "$upload_id") }
        }));

        pipeline.Add(new BsonDocument("$sort", new BsonDocument("_id", 1)));

        if (pageNumber > 1)
            pipeline.Add(new BsonDocument("$skip", (pageNumber - 1) * pageSize));

        pipeline.Add(new BsonDocument("$limit", pageSize));

        var results = database.Aggregate("lesson", pipeline);

        var tagUploads = new Dictionary<string, int[]>();

        foreach (var doc in results)
        {
            var tag = doc["_id"].AsString;
            var uploadIds = doc["uploadIds"].AsBsonArray.Select(u => u.AsInt32).ToArray();
            tagUploads[tag] = uploadIds;
        }

        return tagUploads;
    }


    public Dictionary<int, string> UploaderOptions(BaseOptionsFilter filter)
    {
        var matchStage = new BsonDocument();

        if (!string.IsNullOrEmpty(filter.SearchText))
        {
            var escapedSearchText = JsonSerializer.Serialize(filter.SearchText).Trim('"');
            matchStage = new BsonDocument("$match", new BsonDocument("owner_name", new BsonDocument
            {
                { "$regex", escapedSearchText },
                { "$options", "i" }
            }));
        }

        var groupStage = new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$owner_id" },
            { "owner_name", new BsonDocument("$first", "$owner_name") }
        });

        var sortStage = new BsonDocument("$sort", new BsonDocument("owner_name", 1));

        var skipStage = new BsonDocument("$skip", ((filter.PageNumber ?? 1) - 1) * (filter.PageSize ?? int.MaxValue));
        var limitStage = new BsonDocument("$limit", filter.PageSize ?? int.MaxValue);

        var pipeline = new List<BsonDocument>();

        if (!matchStage.ElementCount.Equals(0))
            pipeline.Add(matchStage);

        pipeline.Add(groupStage);
        pipeline.Add(sortStage);
        pipeline.Add(skipStage);
        pipeline.Add(limitStage);

        var results = database.Aggregate("lesson", pipeline);

        var dict = results
            .Where(doc => doc.Contains("_id") && doc.Contains("owner_name"))
            .ToDictionary(
                doc => doc["_id"].AsInt32,
                doc => doc["owner_name"].AsString
            );

        return dict;
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
    public int[]? OwnerIdInts { get; set; }
    public int? UserId { get; set; }
    public List<int>? UploadIds { get; set; }
    public int? LessonId { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
    public string? SearchText { get; set; }
    public bool? IsStarred { get; set; }
    public int[]? StarredLessons { get; set; }
}