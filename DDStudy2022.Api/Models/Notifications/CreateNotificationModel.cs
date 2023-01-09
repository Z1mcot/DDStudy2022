using DDStudy2022.Api.Models.Users;
using DDStudy2022.DAL.Entities;

namespace DDStudy2022.Api.Models.Notifications
{
    public class CreateNotificationModel
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecieverId { get; set; }
        public string Description { get; set; } = null!;
        public Guid? PostId { get; set; }

        public NotificationTypeEnum NotificationType { get; set; }

    }
}
