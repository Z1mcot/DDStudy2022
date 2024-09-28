using DDStudy2022.Common.Enums;

namespace DDStudy2022.DAL.Entities
{
    public class UserSubscription
    {
        public Guid Id { get; init; }
        public Guid AuthorId { get; init; }
        public virtual User Author { get; init; } = null!;
        public Guid SubscriberId { get; init; }
        public virtual User Subscriber { get; init; } = null!;
        public DateTimeOffset SubscriptionDate { get; init; }
        
        public SubscriptionStatus Status { get; set; }
    }
}
