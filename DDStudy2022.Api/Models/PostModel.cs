namespace DDStudy2022.Api.Models
{
    // Просто чтобы в бд постоянно не смотреть какие посты есть
    public class PostModel
    {
        public long Id { get; set; }
        public string? Description { get; set; }
        public List<PostAttachmentModel> Content { get; set; } = null!;
    }
}
