namespace DDStudy2022.Api.Models.Likes
{
    // Используется не только для модификации существующего, но и для создания
    public class ModifyCommentLikeModel
    {
        public Guid? UserId { get; set; }
        public Guid CommentId { get; set; }
    }
}
