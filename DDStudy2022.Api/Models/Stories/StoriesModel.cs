using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Users;

namespace DDStudy2022.Api.Models.Stories
{
    public class StoriesModel
    {
        public Guid Id { get; set; }
        public UserAvatarModel Author { get; set; } = null!;
        public AttachmentExternalModel Content { get; set; } = null!;
        public DateTimeOffset PublishDate { get; set; }
    }
}
