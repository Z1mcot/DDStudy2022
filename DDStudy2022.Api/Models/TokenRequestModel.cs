namespace DDStudy2022.Api.Models
{
    public class TokenRequestModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenRequestModel
    {
        public string RefreshToken { get; set; }
    }
}
