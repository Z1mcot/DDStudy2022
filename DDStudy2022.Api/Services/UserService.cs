using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDStudy2022.Api.Configs;
using DDStudy2022.Api.Models.Attachments;
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
        private readonly AuthService _authService;
        private Func<UserModel, string?>? _linkGenerator;
        public void SetLinkGenerator(Func<UserModel, string?> linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public UserService(IMapper mapper, DataContext context, AuthService authService)
        {
            _mapper = mapper;
            _context = context;
            _authService = authService;
        }

        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<DAL.Entities.User>(model);
            var t = await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
            return t.Entity.Id;
        }

        public async Task<IEnumerable<UserAvatarModel>> GetUsers()
        {
            var users = await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
            return users.Select(x => new UserAvatarModel(x, _linkGenerator));
        }

        private async Task<User> GetUserById(Guid userId)
        {
            var userEntity = await _context.Users.Include(u => u.Avatar).FirstOrDefaultAsync(u => u.Id == userId);
            if (userEntity == null)
                throw new Exception("user not found");

            return userEntity;
        }

        public async Task<UserAvatarModel> GetUserModel(Guid userId)
        {
            var user = await GetUserById(userId);

            return new UserAvatarModel(_mapper.Map<UserModel>(user), user.Avatar == null ? null : _linkGenerator);
        }

        /*public async Task<DAL.Entities.User> GetUserWithPosts(Guid userId)
        {
            var user = await _context.Users.Include(u => u.Posts).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new Exception("user not found");
            return user;
        }*/

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

        public async Task AddAvatarToUser(Guid userId, MetadataModel meta, string filePath)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var avatar = new Avatar { Author = user, MimeType = meta.MimeType, FilePath = filePath, Name = meta.Name, Size = meta.Size };
                user.Avatar = avatar;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<AttachmentModel> GetUserAvatar(Guid userId)
        {
            var user = await GetUserById(userId);
            var attachment = _mapper.Map<AttachmentModel>(user.Avatar);

            return attachment;
        }

        // SuspendUser вместо DeleteUser
        public async Task SuspendUser(Guid userId)
        {
            var user = await GetUserById(userId);
            if (user == null)
                throw new Exception("user not found");
            // Блокируем аккаунт юзера
            user.IsActive = false;

            // И деактивируем все сессии.
            var allActiveSessions = await _authService.GetUserSessions(user.Id);
            foreach (var session in allActiveSessions)
                await _authService.DeactivateSession(session.RefreshToken);

            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<SessionModel>> GetUserSessionModels(Guid userId)
            => _mapper.Map<List<SessionModel>>(await _authService.GetUserSessions(userId));
        

    }
}
