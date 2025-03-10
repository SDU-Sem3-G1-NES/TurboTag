namespace API.DTOs
{
    public class LibraryDTO
    {
        public required int libraryId { get; set; }
        public required string libraryName { get; set; }
        public LibraryDTO(int libraryId, string libraryName)
        {
            this.libraryId = libraryId;
            this.libraryName = libraryName;
        }
    }
}   