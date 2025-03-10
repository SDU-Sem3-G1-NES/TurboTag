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
        public FileMetadataDTO(int uploadId, string fileType, string fileName, float fileSize, int? duration, string uploadDate, string checkSum)
        {
            this.uploadId = uploadId;
            this.fileType = fileType;
            this.fileName = fileName;
            this.fileSize = fileSize;
            this.duration = duration;
            this.uploadDate = uploadDate;
            this.checkSum = checkSum;
        }
    }
}