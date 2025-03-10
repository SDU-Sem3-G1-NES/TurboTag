namespace API.DTOs
{
    public class FileMetadataDTO
    {
        public int uploadId { get; set; }
        public string fileType { get; set; }
        public string fileName { get; set; }
        public float fileSize { get; set; }
        public int? duration { get; set; }
        public string uploadDate { get; set; }
        public string checkSum { get; set; }
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