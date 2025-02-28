namespace API.Models
{
    public class Metadata
    {
        private int _id { get; set; }
        private string? fileName { get; set; }
        private string? fileType { get; set; }
        private string? size { get; set; }
        private DateTime uploadDate { get; set; }
        private string? checksum { get; set; }
        private int duration { get; set; } // in minutes
        private string? resolution { get; set; }
    }
}
