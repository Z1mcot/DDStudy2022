namespace DDStudy2022.Api.Models.Users
{
    public class UserModel
    {
        public UserModel()
        {
            Id = default;
            NameTag = "@deadAcc";
            Name = "Deleted";
        }

        public Guid Id { get; set; }
        public string NameTag { get; set; } = null!;
        public string Name { get; set; } = null!;

    }
}
