namespace API.DTOs
{
    public class DescriptionDTO
    {
        public string summary { get; set; }
        public List<string> tags { get; set; }
        public DescriptionDTO(string summary, List<string> tags)
        {
            this.summary = summary;
            this.tags = tags;
        }
    }
}