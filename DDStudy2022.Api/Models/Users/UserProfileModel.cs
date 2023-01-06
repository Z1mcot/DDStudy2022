namespace DDStudy2022.Api.Models.Users
{
    public class UserProfileModel : UserAvatarModel
    {
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public uint SubscriptionsCount { get; set; }
        public uint SubscribersCount { get; set; }
        public uint PostsCount { get; set; }
        public uint isPrivate { get; set; }
    }
}
