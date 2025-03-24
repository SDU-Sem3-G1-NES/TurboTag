namespace API.DTOs;

public class DescriptionDTO(string summary, List<string> tags)
{
    public string Summary { get; set; } = summary;
    public List<string> Tags { get; set; } = tags;
}