namespace API.DTOs
{
    public class LibraryDTO
    {
        public int libraryId { get; set; }
        public string libraryName { get; set; }
        public LibraryDTO(int libraryId, string libraryName)
        {
            this.libraryId = libraryId;
            this.libraryName = libraryName;
        }
    }
}   