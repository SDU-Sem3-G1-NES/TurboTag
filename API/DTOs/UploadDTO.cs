using System.Text.Json.Serialization;

namespace API.DTOs;

[method: JsonConstructor]
public class UploadDto(int id, int ownerId, DateTime date, string type, int libraryId)
{
    public int Id { get; set; } = id;
    public int OwnerId { get; set; } = ownerId;
    public DateTime Date { get; set; } = date;
    public string Type { get; set; } = type;
    public int LibraryId { get; set; } = libraryId;
}