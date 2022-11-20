namespace DDStudy2022.Api.Models.Subscriptions
{
    public class UnsubscribeRequest
    {
        public Guid? SubscriberId { get; set; }
        public Guid AuthorId { get; set; }
    }
}
