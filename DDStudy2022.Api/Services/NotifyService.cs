using AutoMapper;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Api.Models.Notifications;
using DDStudy2022.Api.Models.Pushes;
using DDStudy2022.Common.Enums;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace DDStudy2022.Api.Services
{
    public class NotifyService
    {
        private readonly GooglePushService _googlePushService;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public NotifyService(GooglePushService googlePushService, DataContext context, IMapper mapper)
        {
            _googlePushService = googlePushService;
            _context = context;
            _mapper = mapper;
        }

        public async Task SendNewPostNotification(Post post)
        {
            var pushSubscribers = await _context.Subscriptions.Include(s => s.Subscriber)
                                                              .AsNoTracking()
                                                              .Where(s => s.AuthorId == post.AuthorId && s.Subscriber.PushToken != null)
                                                              .Select(s => s.Subscriber.PushToken!)
                                                              .ToListAsync();

            var model = new PushModel 
            {
                Alert = new()
                {
                    Title = "New post",
                    Body = $"{post.Author.Name} just posted. Check it out!",
                }
            };

            _googlePushService.SendNotification(pushSubscribers, model);

        }

        public async Task SendNewCommentNotification(PostComment comment)
        {
            var pushSubscriber = await _context.Posts.Include(p => p.Author)
                                                     .AsNoTracking()
                                                     .SingleOrDefaultAsync(p => p.Id == comment.PostId && p.Author.PushToken != null)
                                ?? throw new UserNotFoundException();

            var model = new PushModel
            {
                Alert = new()
                {
                    Title = $"New comment from {comment.Author.Name}",
                    Body = $"{comment.Author.Name} left a comment under your post. Check it out!",
                }
            };

            _googlePushService.SendNotification(pushSubscriber.Author.PushToken!, model);
        }

        private async Task AddNotificationToDb(Guid senderId, Guid recieverId, string notificationObjectDescription, NotificationTypeEnum type, Guid notificationObjectId)
        {
            var createNotifyModel = new CreateNotificationModel()
            {
                SenderId = senderId,
                RecieverId = recieverId,
                Description = notificationObjectDescription,
                NotificationType = type,
                NotificationObjectId = notificationObjectId,
            };

            var dbNotify = _mapper.Map<Notification>(createNotifyModel);
            await _context.AddAsync(dbNotify);
            await _context.SaveChangesAsync();
        }
    }
}
