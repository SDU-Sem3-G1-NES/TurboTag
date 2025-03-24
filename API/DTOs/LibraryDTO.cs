namespace API.DTOs;

public class LibraryDTO(int libraryId, string libraryName)
{
    public int Id { get; set; } = libraryId;
    public string Name { get; set; } = libraryName;
}