using System.Collections.Generic;

namespace API.DTOs
{
    public class UploadDetailsDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public List<string> Tags { get; set; }

        public UploadDetailsDTO(int id, string description, string title, List<string> tags)
        {
            Id = id;
            Description = description;
            Title = title;
            Tags = tags;
        }
    }
}