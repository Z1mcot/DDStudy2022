using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.DAL.Entities;

namespace DDStudy2022.Api.Services
{
    public class LinkGeneratorService
    {
        public Func<PostAttachment, string?>? LinkContentGenerator;
        public Func<User, string?>? LinkAvatarGenerator;
        public void FixAvatar(User src, UserAvatarModel dest)
        {
            dest.AvatarLink = src.Avatar == null ? null : LinkAvatarGenerator?.Invoke(src);
        }
        public void FixContent(PostAttachment src, AttachmentExternalModel dest)
        {
            dest.ContentLink = LinkContentGenerator?.Invoke(src);
        }
    }
}
