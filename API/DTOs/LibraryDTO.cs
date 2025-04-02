namespace API.DTOs;

public class LibraryDto(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
}