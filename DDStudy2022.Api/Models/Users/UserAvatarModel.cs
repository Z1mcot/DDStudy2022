namespace DDStudy2022.Api.Models.Users
{
    public class UserAvatarModel : UserModel
    {
        public string? AvatarLink { get; set; }
        public UserAvatarModel(UserModel model, Func<UserModel, string?>? linkGenerator)
        {
            Id = model.Id;
            Name = model.Name;
            Email = model.Email;
            BirthDate = model.BirthDate;
            AvatarLink = linkGenerator?.Invoke(model);
        }
    }
}
