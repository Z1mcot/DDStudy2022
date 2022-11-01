using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDStudy2022.Api.Configs;
using DDStudy2022.Api.Models;
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


        private async Task<DAL.Entities.User> GetUserById(Guid userId)
        {
            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userEntity == null)
                throw new Exception("user not found");
            return userEntity;
        }

        public async Task ChangeUserPassword(Guid userId, string OldPassword, string newPassword)
        {
            var user = await GetUserById(userId);
            if (!HashHelper.Verify(OldPassword, user.PasswordHash))
                throw new Exception("Wrong password");

            user.PasswordHash = HashHelper.GetHash(newPassword);
            _context.SaveChanges();
        }

        public async Task<UserModel> GetUser(Guid userId)
        {
            var user = await GetUserById(userId);

            return _mapper.Map<UserModel>(user);
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

        public TokenModel GenerateTokens(DAL.Entities.User user)
        {
            var dtNow = DateTime.Now;

            var accessJwt = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim("displayName", user.Name),
                    new Claim("id", user.Id.ToString()),
                },
                expires: DateTime.Now.AddMinutes(_config.Lifetime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256)
                );
            var encodedAccessJwt = new JwtSecurityTokenHandler().WriteToken(accessJwt);

            var refreshJwt = new JwtSecurityToken(
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim("id", user.Id.ToString()),
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
            return GenerateTokens(user);
        }

        public async Task<TokenModel> RenewToken(string refreshToken)
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
                || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256 ,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("invalid token");
            }

            if(principal.Claims.FirstOrDefault(p => p.Type == "id")?.Value is String userIdString && Guid.TryParse(userIdString, out var userId)) 
            {
                var user = await GetUserById(userId);
                return GenerateTokens(user);
            }
            else
            {
                throw new SecurityTokenException("invalid token");
            }
        }

    }
}
