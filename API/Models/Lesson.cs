namespace API.Models
{
    public class Lesson
    {
        private int _id { get; set; }

        private string tittle { get; set; }
        private string? description { get; set; }
        private string[]? tags { get; set; }
        private string[]? category { get; set; }
        private string? filetype { get; set; }
    }
}
