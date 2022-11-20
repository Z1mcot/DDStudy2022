namespace DDStudy2022.Api.Models.Users
{
    public class UserProfileModel : UserAvatarModel
    {
        public uint SubscriptionsCount { get; set; }
        public uint SubscribersCount { get; set; }
        public uint PostsCount { get; set; }
    }
}
