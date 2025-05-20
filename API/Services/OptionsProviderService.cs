using Api.Controllers;
using API.Controllers;
using API.Repositories;

namespace API.Services;

public interface IOptionsProviderService : IServiceBase
{
    public IEnumerable<OptionDto> GetTagOptions(TagOptionsFilter filter);
}

public class OptionsProviderProviderService(ILessonService lessonService) : IOptionsProviderService
{
    public IEnumerable<OptionDto> GetTagOptions(TagOptionsFilter filter)
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
}