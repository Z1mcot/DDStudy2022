namespace DDStudy2022.Api.Models.Posts
{
    public class PostLikeModel
    {
        public Guid? UserId { get; set; }
        public Guid PostId { get; set; }
    }
}
