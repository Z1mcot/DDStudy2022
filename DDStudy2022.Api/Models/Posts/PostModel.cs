using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Users;

namespace DDStudy2022.Api.Models.Posts
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public UserAvatarModel Author { get; set; } = null!;
        public List<AttachmentWithLinkModel> Content { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        public bool IsModified { get; set; } = false;
    }
}
