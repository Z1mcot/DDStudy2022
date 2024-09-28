using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DDStudy2022.Common;

namespace DDStudy2022.Api.Configs
{
    public class AuthConfig
    {
        public const string ConfigPosition = "Auth";
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        
        public string PrivateKey { get; init; } = string.Empty;
        // public string PublicKey { get; init; } = string.Empty;
        public int Lifetime { get; init; }
        public SecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PrivateKey));
        
        // public SecurityKey TestKey => new RsaSecurityKey(EncryptionHelper.GetRsa(PrivateKey, PublicKey));
    }
}
