namespace API.DTOs
{
    public class FileMetadataDTO
    {
        public int Id { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public float FileSize { get; set; }
        public int Duration { get; set; }
        public string Date { get; set; }
        public string CheckSum { get; set; }

        public FileMetadataDTO(int id, string fileType, string fileName, float fileSize, int duration, string date, string checkSum)
        {
            Id = id;
            FileType = fileType;
            FileName = fileName;
            FileSize = fileSize;
            Duration = duration;
            Date = date;
            CheckSum = checkSum;
        }
    }
}