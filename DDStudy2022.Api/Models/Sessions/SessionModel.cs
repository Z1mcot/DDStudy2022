namespace DDStudy2022.Api.Models.Sessions
{
    public class SessionModel
    {
        public Guid RefreshToken { get; set; }
        public string? IPAddress { get; set; }
        public DateTime Created { get; set; }
    }
}
