namespace API.DTOs;

public class TagDto
{
    public TagDto()
    {
        TagId = 0;
        TagName = "";
    }

    public TagDto(int tagId, string tagName)
    {
        TagId = tagId;
        TagName = tagName;
    }

    public int TagId { get; set; }
    public string TagName { get; set; }
}