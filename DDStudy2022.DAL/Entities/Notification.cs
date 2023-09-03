using DDStudy2022.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{

    public class Notification
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public virtual User Sender { get; set; } = null!;
        public Guid RecieverId { get; set; }
        public virtual User Reciever { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTimeOffset NotifyDate { get; set; }

        public NotificationTypeEnum NotificationType;

        public Guid? PostId { get; set; }
        public virtual Post? Post { get; set; }
    }
}
