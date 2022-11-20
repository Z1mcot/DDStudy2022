using DDStudy2022.Api.Models.Users;

namespace DDStudy2022.Api.Models.Comments
{
    public class CommentModel
    {
        public UserAvatarModel Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime PublishDate { get; set; }
    }
}
