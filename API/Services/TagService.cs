using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface ITagService : IServiceBase
{
    IEnumerable<TagDto> GetAllTags(TagFilter? filter);
    bool AddTag(TagDto tag);
    void DeleteTags(TagFilter? filter);
}

public class TagService(ITagRepository tagRepository) : ITagService
{
    public IEnumerable<TagDto> GetAllTags(TagFilter? filter)
    {
        return tagRepository.GetAllTags(filter);
    }

    public bool AddTag(TagDto tag)
    {
        return tagRepository.AddTag(tag);
    }

    public void DeleteTags(TagFilter? filter)
    {
        tagRepository.DeleteTags(filter);
    }
}