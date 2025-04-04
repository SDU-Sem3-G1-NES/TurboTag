using System.Text.Json.Serialization;

namespace API.Dtos;

public class UploadDetailsDto
{
    public UploadDetailsDto()
    {
        Description = "";
        Title = "";
        Tags = new List<string>();
    }

    [method: JsonConstructor]
    public UploadDetailsDto(int id, string description, string title, List<string> tags)
    {
        Id = id;
        Description = description;
        Title = title;
        Tags = tags;
    }

    public int Id { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public List<string> Tags { get; set; }
}