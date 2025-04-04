using System.Text.Json.Serialization;

namespace API.Dtos;

public class UploadDto
{
    public UploadDto()
    {
        Date = new DateTime();
        Type = "";
    }


    [method: JsonConstructor]
    public UploadDto(int id, int ownerId, DateTime date, string type, int libraryId)
    {
        Id = id;
        OwnerId = ownerId;
        Date = date;
        Type = type;
        LibraryId = libraryId;
    }

    public int Id { get; set; }
    public int OwnerId { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public int LibraryId { get; set; }
}