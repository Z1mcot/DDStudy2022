namespace DDStudy2022.Api.Models
{
    public class CommentModel
    {
        public string Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime PublishDate { get; set; }
    }
}
