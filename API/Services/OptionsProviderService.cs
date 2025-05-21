using Api.Controllers;
using API.Controllers;
using API.Repositories;

namespace API.Services;

public interface IOptionsProviderService : IServiceBase
{
    IEnumerable<OptionDto> GetTagOptions(BaseOptionsFilter filter);
    IEnumerable<OptionDto> GetUploaderOptions(BaseOptionsFilter filter);
}

public class OptionsProviderProviderService(ILessonService lessonService)
    : IOptionsProviderService
{
    public IEnumerable<OptionDto> GetTagOptions(BaseOptionsFilter filter)
    {
        var tags = lessonService.TagOptions(filter);
        if (filter.UserId is not null)
        {
            var userOwnedLessons = lessonService.GetAllLessons(new LessonFilter
            {
                OwnerId = filter.UserId
            }).Select(l => l.UploadId);

            var userStarredLessons = lessonService.GetAllLessons(new LessonFilter
            {
                IsStarred = true,
                UserId = filter.UserId
            }).Select(l => l.UploadId);

            return tags
                .Where(x => x.Value.Any(v => userOwnedLessons.Contains(v) || userStarredLessons.Contains(v)))
                .Select(x => new OptionDto
                {
                    DisplayText = x.Key,
                    Value = x.Key
                });
        }

        return tags
            .Select(x => new OptionDto
            {
                DisplayText = x.Key,
                Value = x.Key
            });
    }

    public IEnumerable<OptionDto> GetUploaderOptions(BaseOptionsFilter filter)
    {
        var uploaders = lessonService.UploaderOptions(filter); // Dictionary<int, string>

        if (filter.UserId is not null)
        {
            var userOwnedLessons = lessonService.GetAllLessons(new LessonFilter
            {
                OwnerId = filter.UserId
            }).Select(l => l.OwnerId);

            var userStarredLessons = lessonService.GetAllLessons(new LessonFilter
            {
                IsStarred = true,
                UserId = filter.UserId
            }).Select(l => l.OwnerId);

            return uploaders
                .Where(x => userOwnedLessons.Contains(x.Key) || userStarredLessons.Contains(x.Key))
                .Select(x => new OptionDto
                {
                    Value = x.Key.ToString(),
                    DisplayText = x.Value
                });
        }

        return uploaders
            .Select(x => new OptionDto
            {
                Value = x.Key.ToString(),
                DisplayText = x.Value
            });
    }
}