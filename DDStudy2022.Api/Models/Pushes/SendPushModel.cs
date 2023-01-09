namespace DDStudy2022.Api.Models.Pushes
{
    public class SendPushModel
    {
        public Guid? UserId { get; set; }
        public PushModel Push { get; set; } = null!;
    }
}
