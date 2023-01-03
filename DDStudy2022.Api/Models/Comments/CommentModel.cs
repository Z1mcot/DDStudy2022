using DDStudy2022.Api.Models.Users;

namespace DDStudy2022.Api.Models.Comments
{
    public class CommentModel
    {
        public Guid Id { get; set; }
        public UserAvatarModel Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public uint Likes { get; set; }
        public bool IsLiked { get; set; }
        public DateTimeOffset PublishDate { get; set; }
    }
}
