namespace DDStudy2022.Api.Models.Users
{
    public class UserProfileModel : UserAvatarModel
    {
        public string Email { get; set; } = "deleted";
        public DateTimeOffset BirthDate { get; set; } = default;
        public uint SubscriptionsCount { get; set; } = 0;
        public uint SubscribersCount { get; set; } = 0;
        public int PostsCount { get; set; } = 0;
        public bool IsPrivate { get; set; }
        public bool IsFollowed { get; set; }
    }
}
