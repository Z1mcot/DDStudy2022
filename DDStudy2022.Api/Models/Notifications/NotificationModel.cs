using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Api.Models.Users;

namespace DDStudy2022.Api.Models.Notifications
{
    public class NotificationModel
    {
        public Guid Id { get; set; }
        public UserAvatarModel Sender { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTimeOffset NotifyDate { get; set; }
        public Guid? PostId { get; set; }
    }
}
