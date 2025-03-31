using System.Text.Json.Serialization;

namespace API.Dtos;

[method: JsonConstructor]
public class UploadDetailsDto(int id, string description, string title, List<string> tags)
{
    public int Id { get; set; } = id;
    public string Description { get; set; } = description;
    public string Title { get; set; } = title;
    public List<string> Tags { get; set; } = tags;
}