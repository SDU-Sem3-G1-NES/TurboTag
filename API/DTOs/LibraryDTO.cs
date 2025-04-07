namespace API.DTOs;

public class LibraryDto
{
    public LibraryDto()
    {
        Name = "";
    }

    public LibraryDto(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
}