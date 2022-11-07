namespace DDStudy2022.Api.Models
{
    public class PostModel
    {
        public long Id { get; set; } // Просто чтобы в бд постоянно не смотреть какие посты есть
        public string? Description { get; set; }
        public List<PostAttachmentModel> Content { get; set; } = null!;
        public DateTime PublishDate { get; set; }
    }
}
