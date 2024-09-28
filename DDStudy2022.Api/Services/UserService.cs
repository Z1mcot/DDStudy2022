using AutoMapper;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Notifications;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Common;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDStudy2022.Api.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public UserService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<User>(model);
            var t = await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
            return t.Entity.Id;
        }

        public async Task ChangeUserPassword(Guid userId, PasswordChangeRequest request)
        {
            var user = await GetUserById(userId);
            if (!HashHelper.Verify(request.OldPassword, user.PasswordHash))
                throw new ChangePasswordException();

            user.PasswordHash = HashHelper.GetHash(request.NewPassword);
            await _context.SaveChangesAsync();
        }

        public async Task ModifyUserInfo(Guid userId, ModifyUserInfoRequest request)
        {
            var user = await GetUserById(userId);
            
            user.Name = request.Name ?? user.Name;
            user.NameTag = request.NameTag ?? user.NameTag;
            user.Email = request.Email ?? user.Email;
            user.BirthDate = request.BirthDate ?? user.BirthDate;

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

        //public async Task UnsuspendUser(Guid userId)
        //{
        //    // Когда-нибудь
        //}

        public async Task SuspendUser(Guid userId)
        {
            var user = await GetUserById(userId);

            user.IsActive = false;
            await DeactivateAllUserSessions(userId);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserAvatarModel>> GetUsers() 
            => await _context.Users
            .Include(x => x.Avatar)
            .AsNoTracking()
            .Where(u => u.IsActive)
            .Select(x => _mapper.Map<UserAvatarModel>(x)).ToListAsync();

        public async Task<UserProfileModel> GetUserModel(Guid userId, Guid? requesterId = null)
        {
            var user = await _context.Users.Include(u => u.Avatar)
                                           .Include(u => u.Subscribers)
                                           .Include(u => u.Posts)
                                           .Include(u => u.Subscriptions)
                                           .FirstOrDefaultAsync(u => u.Id == userId) 
                       ?? throw new UserNotFoundException();
                
            return _mapper.Map<User, UserProfileModel>(user, 
                opts: o => o.AfterMap((src, dest) => dest.IsFollowed = src.Subscribers?.Any(s => s.SubscriberId == requesterId) ?? false)
                ); // TODO replace int with bool
        }

        public async Task<ICollection<SessionModel>> GetUserSessionModels(Guid userId)
            => _mapper.Map<List<SessionModel>>(await GetUserSessions(userId));

        public async Task<ICollection<UserSession>> GetUserSessions(Guid userId) 
            => await _context.UserSessions.Where(s => s.UserId == userId && s.IsActive).ToListAsync();

        public async Task DeactivateSession(Guid sessionId)
        {
            var session = await GetSessionById(sessionId);
            session.IsActive = false;

            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAllUserSessions(Guid userId)
        {
            var allActiveSessions = await GetUserSessions(userId);
            foreach (var session in allActiveSessions)
                await DeactivateSession(session.RefreshToken);
        }

        public async Task ChangeAccountPrivacySetting(Guid userId)
        {
            var user = await GetUserById(userId);
            user.IsPrivate = !user.IsPrivate;

            await _context.SaveChangesAsync();
        }

        public async Task<List<UserAvatarModel>> SearchUsers(string nameTag, int skip, int take)
        {
            return await _context.Users.Include(u => u.Avatar)
                                       .AsNoTracking()
                                       .Where(u => u.NameTag.ToLower().StartsWith(nameTag.ToLower()) && u.IsActive)
                                       .Select(u => _mapper.Map<UserAvatarModel>(u))
                                       .Skip(skip).Take(take)
                                       .ToListAsync();
        }

        public async Task SetPushToken(Guid userId, string? token = null)
        {
            var user = await GetUserById(userId);
            user.PushToken = token;
            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetPushToken(Guid userId)
        {
            var user = await GetUserById(userId);
            return user.PushToken;
        }

        public async Task<List<NotificationModel>> GetNotifications(Guid userId, int skip, int take)
        {
            return await _context.Notifications
                                 .Include(n => n.Sender).ThenInclude(u => u.Avatar)
                                 .AsNoTracking()
                                 .Where(n => n.RecieverId == userId)
                                 .OrderByDescending(nm => nm.NotifyDate)
                                 .Skip(skip).Take(take)
                                 .Select(n => _mapper.Map<NotificationModel>(n))
                                 .ToListAsync();
        }


        public async Task<User> GetUserById(Guid userId) 
            => await _context.Users.Include(u => u.Avatar).FirstOrDefaultAsync(u => u.Id == userId && u.IsActive)
                ?? throw new UserNotFoundException();

        private async Task<UserSession> GetSessionByRefreshToken(Guid id) 
            => await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshToken == id)
                ?? throw new SessionNotFoundException();
        
        private async Task<UserSession> GetSessionById(Guid sessionId)
            => await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == sessionId && x.IsActive) 
                ?? throw new SessionNotFoundException();
    }
}
