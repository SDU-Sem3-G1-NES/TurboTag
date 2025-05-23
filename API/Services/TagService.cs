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
        var added = tagRepository.AddTag(tag);
        if (!added)
            return false;

        var allTags = tagRepository.GetAllTags();

        var csvPath = Path.Combine("..", "setup", "python", "tags.csv");
        using var writer = new StreamWriter(csvPath, false);
        writer.WriteLine("Tag ID,Tag Name");
        foreach (var t in allTags.OrderBy(t => t.TagId)) writer.WriteLine($"{t.TagId},{t.TagName}");

        return true;
    }


    public void DeleteTags(TagFilter? filter)
    {
        tagRepository.DeleteTags(filter);
    }
}