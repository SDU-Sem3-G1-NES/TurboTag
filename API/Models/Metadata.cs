namespace API.Models
{
    public class Metadata
    {
        private int _id { get; set; }
        private string? filename { get; set; }
        private string? filetype { get; set; }
        private string? mimetype { get; set; }
        private string? size { get; set; }
        private string? storagePath { get; set; }
        private DateTime uploadDate { get; set; }
        private int ownerId { get; set; }
        private string? checksum { get; set; }
        private int duration { get; set; } // in minutes
        private string? resolution { get; set; }
        private int pageCount { get; set; } // for PDFs
        private int bitRate { get; set; } // for videos
        private string? encoding { get; set; } // for videos
        private string? previewUrl { get; set; } // for thumbnails
    }
}
