using AutoMapper;
using DDStudy2022.Api.Configs;
using DDStudy2022.Api.Models.Tokens;
using DDStudy2022.Common;
using DDStudy2022.Common.Consts;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DDStudy2022.Api.Services
{
    public class AuthService
    {
        private readonly DataContext _context;
        private readonly AuthConfig _config;

        public AuthService(DataContext context, IOptions<AuthConfig> authConfig)
        {
            _context = context;
            _config = authConfig.Value;
        }

        private async Task<DAL.Entities.User> GetUserByCredentials(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == login.ToLower());
            if (user == null)
                throw new Exception("User is not present in database");

            if (!HashHelper.Verify(password, user.PasswordHash))
                throw new Exception("Wrong password");

            return user;
        }

        public async Task<UserSession> GetSessionByRefreshToken(Guid id)
        {
            var session = await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshToken == id);
            if (session == null)
                throw new Exception("session not found");
            return session;
        }

        public TokenModel GenerateTokens(DAL.Entities.UserSession session)
        {
            var dtNow = DateTime.Now;
            if (session.User == null)
                throw new Exception("somehow we managed to create session without user");
            if (!session.User.IsActive)
                throw new Exception("your account has been suspended");

            var accessJwt = new JwtSecurityToken(
                    issuer: _config.Issuer,
                    audience: _config.Audience,
                    notBefore: dtNow,
                    claims: new Claim[]
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, session.User.Name),
                        new Claim(ClaimNames.SessionId, session.Id.ToString()),
                        new Claim(ClaimNames.Id, session.User.Id.ToString()),
                    },
                    expires: DateTime.Now.AddMinutes(_config.Lifetime),
                    signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256)
                );
            var encodedAccessJwt = new JwtSecurityTokenHandler().WriteToken(accessJwt);

            var refreshJwt = new JwtSecurityToken(
                    notBefore: dtNow,
                    claims: new Claim[]
                    {
                        new Claim(ClaimNames.RefreshToken, session.RefreshToken.ToString()),
                    },
                    expires: DateTime.Now.AddHours(_config.Lifetime),
                    signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256)
                );
            var encodedRefreshJwt = new JwtSecurityTokenHandler().WriteToken(refreshJwt);

            return new TokenModel(encodedAccessJwt, encodedRefreshJwt);
        }

        public async Task<TokenModel> GetToken(string login, string password)
        {
            var user = await GetUserByCredentials(login, password);
            var session = await _context.UserSessions.AddAsync(new DAL.Entities.UserSession
            {
                Id = Guid.NewGuid(),
                User = user,
                RefreshToken = Guid.NewGuid(),
                Created = DateTime.UtcNow,
            });

            await _context.SaveChangesAsync();
            return GenerateTokens(session.Entity);
        }

        public async Task<TokenModel> GetTokenByRefreshToken(string refreshToken)
        {
            var validParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = _config.SymmetricSecurityKey
            };
            var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validParams, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken
                || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("invalid token");
            }

            if (principal.Claims.FirstOrDefault(p => p.Type == "refreshToken")?.Value is string refreshIdString
                && Guid.TryParse(refreshIdString, out var refreshId))
            {
                var session = await GetSessionByRefreshToken(refreshId);
                if (!session.IsActive)
                {
                    throw new Exception("session timed out");
                }

                var user = session.User;

                session.RefreshToken = Guid.NewGuid();
                await _context.SaveChangesAsync();

                return GenerateTokens(session);
            }
            else
            {
                throw new SecurityTokenException("invalid token");
            }
        }

        public async Task<ICollection<UserSession>> GetUserSessions(Guid userId)
        {
            var sessions = await _context.UserSessions.Where(s => s.UserId == userId && s.IsActive).ToListAsync();
            if (sessions == null)
                throw new Exception("No active sessions for this user");

            return sessions;
        }

        public async Task<UserSession> GetSessionById(Guid id)
        {
            var session = await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (session == null)
                throw new Exception("session not found");
            return session;
        }

        public async Task DeactivateSession(Guid refreshToken)
        {
            var session = await GetSessionByRefreshToken(refreshToken);
            session.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}
