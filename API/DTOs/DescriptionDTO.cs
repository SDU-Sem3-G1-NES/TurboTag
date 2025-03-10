namespace API.DTOs
{
    public class DescriptionDTO
    {
        public required string summary { get; set; }
        public required List<string> tags { get; set; }
    }
}