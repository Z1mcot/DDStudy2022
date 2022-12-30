namespace DDStudy2022.Api.Models.Tokens
{
    public class TokenRequestModel
    {
        public TokenRequestModel(string login, string password, string? ip)
        {
            Login = login;
            Password = password;
            Ip = ip;
        }

        public string Login { get; set; }
        public string Password { get; set; }
        public string? Ip { get; set; }
    }

    public class RefreshTokenRequestModel
    {
        public RefreshTokenRequestModel(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        public string RefreshToken { get; set; }
    }
}
