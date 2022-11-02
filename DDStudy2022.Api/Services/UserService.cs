using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDStudy2022.Api.Configs;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Tokens;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Common;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DDStudy2022.Api.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly AuthConfig _config;

        public UserService(IMapper mapper, DataContext context, IOptions<AuthConfig> authConfig)
        {
            _mapper = mapper;
            _context = context;
            _config = authConfig.Value;
        }

        public async Task CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<DAL.Entities.User>(model);
            await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
        }
        public async Task<List<UserModel>> GetUsers()
        {
            // NoTracking не следит за изменениями тех сущностей которые мы вернули по нашему запросу. Удобно если мы только читаем
            return await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
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

        private async Task<DAL.Entities.User> GetUserById(Guid userId)
        {
            var userEntity = await _context.Users.Include(x => x.Sessions).FirstOrDefaultAsync(u => u.Id == userId);
            if (userEntity == null)
                throw new Exception("user not found");

            return userEntity;
        }

        public async Task<UserModel> GetUser(Guid userId)
        {
            var user = await GetUserById(userId);

            return _mapper.Map<UserModel>(user);
        }

        public async Task ChangeUserPassword(Guid userId, string OldPassword, string newPassword)
        {
            var user = await GetUserById(userId);
            if (!HashHelper.Verify(OldPassword, user.PasswordHash))
                throw new Exception("Wrong password");

            user.PasswordHash = HashHelper.GetHash(newPassword);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckUserExistence(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task DeleteUser(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ICollection<SessionModel>> GetUserSessions(Guid userId)
        {
            var user = await GetUserById(userId);
            if (user.Sessions == null)
                throw new Exception("No active sessions for this user");

            return _mapper.Map<List<SessionModel>>(user.Sessions);
        }

        public async Task DeactivateSession(Guid refreshToken)
        {
            var session = await GetSessionByRefreshToken(refreshToken);
            session.IsActive = false;

            await _context.SaveChangesAsync();
        }

        public async Task<UserSession> GetSessionById(Guid id)
        {
            var session = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == id);
            if (session == null)
                throw new Exception("session not found");
            return session;
        }

        private async Task<UserSession> GetSessionByRefreshToken(Guid id)
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

            var accessJwt = new JwtSecurityToken(
                    issuer: _config.Issuer,
                    audience: _config.Audience,
                    notBefore: dtNow,
                    claims: new Claim[]
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, session.User.Name),
                        new Claim("sessionId", session.Id.ToString()),
                        new Claim("id", session.User.Id.ToString()),
                    },
                    expires: DateTime.Now.AddMinutes(_config.Lifetime),
                    signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256)
                );
            var encodedAccessJwt = new JwtSecurityTokenHandler().WriteToken(accessJwt);

            var refreshJwt = new JwtSecurityToken(
                    notBefore: dtNow,
                    claims: new Claim[]
                    {
                        new Claim("refreshToken", session.RefreshToken.ToString()),
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

    }
}
