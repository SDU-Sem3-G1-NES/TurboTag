namespace API.DTOs;

public class DescriptionDTO(string summary, List<string> tags)
{
    public string Summary { get; } = summary;
    public List<string> Tags { get; } = tags;
}