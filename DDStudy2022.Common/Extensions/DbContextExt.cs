using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.Common.Extensions
{
    public static class ContextExtension
    {
        public static IQueryable<UserSubscription> IncludeSubscribersWithAvatar(this IQueryable<UserSubscription> subscriptions)
            => subscriptions.Include(s => s.Subscriber).ThenInclude(u => u.Avatar);
        public static IQueryable<Post> IncludeAuthorWithSubscribers(this IQueryable<Post> posts)
            => posts.Include(p => p.Author).ThenInclude(u => u.Subscribers);
        public static IQueryable<Stories> IncludeAuthorWithSubscribers(this IQueryable<Stories> stories)
            => stories.Include(s => s.Author).ThenInclude(u => u.Subscribers);

        public static IQueryable<Post> IncludeAuthorWithAvatar(this IQueryable<Post> posts)
            => posts.Include(p => p.Author).ThenInclude(u => u.Avatar);
        public static IQueryable<PostComment> IncludeAuthorWithAvatar(this IQueryable<PostComment> comments)
            => comments.Include(c => c.Author).ThenInclude(u => u.Avatar);
        public static IQueryable<Stories> IncludeAuthorWithAvatar(this IQueryable<Stories> stories)
            => stories.Include(s => s.Author).ThenInclude(u => u.Avatar);

    }
}
