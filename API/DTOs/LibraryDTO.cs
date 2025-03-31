namespace API.Dtos;

public class LibraryDto(int libraryId, string libraryName)
{
    public int Id { get; set; } = libraryId;
    public string Name { get; set; } = libraryName;
}