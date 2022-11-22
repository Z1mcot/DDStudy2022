namespace DDStudy2022.Api.Models.Likes
{
    // Используется не только для модификации существующего, но и для создания
    public class ModifyPostLikeModel
    {
        public Guid? UserId { get; set; }
        public Guid PostId { get; set; }
    }
}
