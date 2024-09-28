using AutoMapper;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Common.Enums;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using DDStudy2022.Common.Exceptions;

namespace DDStudy2022.Api.Services
{
    public class SubscriptionService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SubscriptionService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task SubscribeToUser(Guid userId, Guid subscribeToUserId)
        {
            var author = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == subscribeToUserId);
            if (author is not { IsActive: true })
                throw new UserNotFoundException();
            
            var subscriptionModel = new UserSubscription
            {
                AuthorId = author.Id,
                SubscriberId = userId,
                Status = author.IsPrivate ? SubscriptionStatus.Pending : SubscriptionStatus.Active,
                SubscriptionDate = DateTime.UtcNow,
            };

            await _context.Subscriptions.AddAsync(subscriptionModel);

            await _context.SaveChangesAsync();
        }

        public async Task UnsubscribeFromUser(Guid userId, Guid unsubscribeFromUserId)
        {
            var sub = await _context.Subscriptions.FirstOrDefaultAsync(s => s.AuthorId == unsubscribeFromUserId && s.SubscriberId == userId)
                ?? throw new SubscriptionNotFoundException();

            _context.Remove(sub);
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmSubscriber(Guid authorId, Guid subscriberId)
        {
            var sub = await _context.Subscriptions.FirstOrDefaultAsync(s => s.AuthorId == authorId && s.SubscriberId == subscriberId)
                ?? throw new SubscriptionRequestNotFoundException();

            sub.Status = SubscriptionStatus.Active;
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserAvatarModel>> GetSubscribers
            (Guid authorId, SubscriptionStatus status, int skip, int take)
        {
            return await _context.Subscriptions
                .Include(s => s.Subscriber).ThenInclude(u => u.Avatar)
                .AsNoTracking()
                .Where(s => s.AuthorId == authorId && s.Status == status)
                .Select(s => s.Subscriber.IsActive ? _mapper.Map<UserAvatarModel>(s.Subscriber) : new UserAvatarModel())
                .Skip(skip).Take(take)
                .ToListAsync();
        }

        public async Task<List<UserAvatarModel>> GetSubscriptions
            (Guid userId, SubscriptionStatus status, int skip, int take)
        {
            return await _context.Subscriptions
                .Include(s => s.Author).ThenInclude(u => u.Avatar)
                .AsNoTracking()
                .Where(s => s.SubscriberId == userId && s.Status == status)
                .Select(s => s.Author.IsActive ? _mapper.Map<UserAvatarModel>(s.Author) : new UserAvatarModel())
                .Skip(skip).Take(take)
                .ToListAsync();
        }
    }
}
