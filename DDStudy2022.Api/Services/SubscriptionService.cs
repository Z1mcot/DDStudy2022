using AutoMapper;
using DDStudy2022.Api.Models.Subscriptions;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using DDStudy2022.Common.Exceptions;
using System.Runtime.CompilerServices;
using DDStudy2022.Common.Extensions;
using Microsoft.AspNetCore.Routing;

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

        public async Task SubscribeToUser(SubscribtionRequest request)
        {
            var author = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == request.AuthorId);
            if (author == null || !author.IsActive)
                throw new UserNotFoundException();

            var dbSub = _mapper.Map<UserSubscription>(request);
            if (!author.IsPrivate)
                dbSub.IsConfirmed = true;

            await _context.Subscriptions.AddAsync(dbSub);

            await _context.SaveChangesAsync();
        }

        public async Task UnsubscribeFromUser(UnsubscribeRequest request)
        {
            var sub = await _context.Subscriptions.FirstOrDefaultAsync(s => s.AuthorId == request.AuthorId && s.SubscriberId == request.SubscriberId);
            if (sub == null)
                throw new SubscriptionNotFoundException();

            _context.Remove(sub);
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmSubscriber(Guid authorId, Guid subscriberId)
        {
            var sub = await _context.Subscriptions.FirstOrDefaultAsync(s => s.AuthorId == authorId && s.SubscriberId == subscriberId);
            if (sub == null)
                throw new SubscriptionRequestNotFoundException();

            sub.IsConfirmed = true;
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserAvatarModel>> GetSubscriptionRequests(Guid authorId)
        {
            return await _context.Subscriptions
                .Include(s => s.Subscriber).ThenInclude(u => u.Avatar)
                .AsNoTracking()
                .Where(s => s.AuthorId == authorId && !s.IsConfirmed)
                .Select(s => _mapper.Map<UserAvatarModel>(s.Subscriber))
                .ToListAsync();
        }

        public async Task<List<UserAvatarModel>> GetSubscribers(Guid authorId)
        {
            return await _context.Subscriptions
                .Include(s => s.Subscriber).ThenInclude(u => u.Avatar)
                .Include(s => s.Author).ThenInclude(u => u.Avatar)
                .AsNoTracking()
                .Where(s => s.AuthorId == authorId && s.IsConfirmed)
                .Select(s => s.Subscriber.IsActive ? _mapper.Map<UserAvatarModel>(s.Subscriber) : new UserAvatarModel())
                .ToListAsync();
        }

        public async Task<List<UserAvatarModel>> GetSubscriptions(Guid userId)
        {
            return await _context.Subscriptions
                .Include(s => s.Author).ThenInclude(u => u.Avatar)
                .Include(s => s.Subscriber).ThenInclude(u => u.Avatar)
                .AsNoTracking()
                .Where(s => s.SubscriberId == userId && s.IsConfirmed)
                .Select(s => s.Author.IsActive ? _mapper.Map<UserAvatarModel>(s.Author) : new UserAvatarModel())
                .ToListAsync();
        }
    }
}
