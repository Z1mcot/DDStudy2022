﻿using AutoMapper;
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

        public async Task ChangeUserPassword(PasswordChangeRequest request)
        {
            var model = _mapper.Map<PasswordChangeModel>(request);

            var user = await GetUserById(model.Id);
            if (!HashHelper.Verify(model.OldPassword, user.PasswordHash))
                throw new Exception("Wrong password"); // TODO

            user.PasswordHash = HashHelper.GetHash(model.NewPassword);
            await _context.SaveChangesAsync();
        }

        public async Task ModifyUserInfo(ModifyUserInfoRequest request)
        {
            var model = _mapper.Map<ModifyUserInfoModel>(request);
            
            var user = await GetUserById(model.Id);
            
            user.Name = model.Name ?? user.Name;
            user.NameTag = model.NameTag ?? user.NameTag;
            user.Email = model.Email ?? user.Email;
            user.BirthDate = model.BirthDate ?? user.BirthDate;

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
                                           .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserNotFoundException();
            var res = _mapper.Map<User, UserProfileModel>(user, 
                opts: opts => opts.AfterMap((src, dest) => dest.isFollowed = src.Subscribers.Any(s => s.SubscriberId == requesterId) ? 1 : 0)
                );
            return res;
        }

        public async Task<ICollection<SessionModel>> GetUserSessionModels(Guid userId)
            => _mapper.Map<List<SessionModel>>(await GetUserSessions(userId));

        public async Task<ICollection<UserSession>> GetUserSessions(Guid userId)
        {
            var sessions = await _context.UserSessions.Where(s => s.UserId == userId && s.IsActive).ToListAsync();
            if (sessions == null)
                throw new SessionNotFoundException();

            return sessions;
        }

        public async Task DeactivateSession(Guid refreshToken)
        {
            var session = await GetSessionByRefreshToken(refreshToken);
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
            user.IsPrivate ^= true;

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

        public async Task AddNotification(string notifyType, Guid senderId, Guid recieverId, string? description, Guid? postId = null)
        {
            var createNotifyModel = new CreateNotificationModel
            {
                SenderId = senderId,
                RecieverId = recieverId,
                Description = description ?? "shouldnt show this",
                NotificationType = NotificationTypeEnum.subscribeNotification
            };
            if (notifyType == "post")
            {
                createNotifyModel.NotificationType = NotificationTypeEnum.postNotification;
                createNotifyModel.PostId = (Guid)postId!;
            }

            var dbNotify = _mapper.Map<Notification>(createNotifyModel);
            await _context.AddAsync(dbNotify);
            await _context.SaveChangesAsync();
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
        {
            var userEntity = await _context.Users.Include(u => u.Avatar).FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
            if (userEntity == null)
                throw new UserNotFoundException();

            return userEntity;
        }

        private async Task<UserSession> GetSessionByRefreshToken(Guid id)
        {
            var session = await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshToken == id);
            if (session == null)
                throw new SessionNotFoundException();
            return session;
        }


    }
}
