namespace API.DTOs;

public class DescriptionDto
{
    public DescriptionDto()
    {
        Summary = "";
        Tags = new List<string>();
    }

    public DescriptionDto(string summary, List<string> tags)
    {
        Summary = summary;
        Tags = tags;
    }

    public string Summary { get; set; }
    public List<string> Tags { get; set; }
}