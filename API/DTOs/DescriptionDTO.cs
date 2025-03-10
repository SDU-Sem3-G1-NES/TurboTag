namespace API.DTOs
{
    public class DescriptionDTO
    {
        public required string summary { get; set; }
        public required List<string> tags { get; set; }
        public DescriptionDTO(string summary, List<string> tags)
        {
            this.summary = summary;
            this.tags = tags;
        }
    }
}