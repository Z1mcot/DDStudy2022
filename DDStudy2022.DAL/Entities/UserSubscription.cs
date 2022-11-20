using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class UserSubscription
    {
        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!; // Спорное имя, но ничего лучше в голову не лезет
        public Guid SubscriberId { get; set; }
        public virtual User Subscriber { get; set; } = null!;
        public DateTimeOffset SubscriptionDate { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
