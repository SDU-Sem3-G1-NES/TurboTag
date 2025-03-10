namespace API.DTOs;
public class LibraryDTO(int libraryId, string libraryName)
{
    public int Id { get; } = libraryId;
    public string Name { get; } = libraryName;
}