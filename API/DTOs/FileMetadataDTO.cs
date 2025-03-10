namespace API.DTOs
{
    public class FileMetadataDTO
    {
        public required int uploadId { get; set; }
        public required string fileType { get; set; }
        public required string fileName { get; set; }
        public required float fileSize { get; set; }
        public int? duration { get; set; }
        public required string uploadDate { get; set; }
        public required string checkSum { get; set; }

    }
}