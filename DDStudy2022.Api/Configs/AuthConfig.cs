using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DDStudy2022.Api.Configs
{
    public class AuthConfig
    {
        public const string ConfigPosition = "Auth";
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int Lifetime { get; set; }
        public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}
