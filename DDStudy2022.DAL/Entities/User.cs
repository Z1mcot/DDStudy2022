using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "not entered";
        public string NameTag { get; set; } = "not entered";
        public string Email { get; set; } = "not entered";
        public string PasswordHash { get; set; } = "not entered";
        public DateTimeOffset BirthDate { get; set; }
        public Avatar? Avatar { get; set; } 

        public virtual ICollection<UserSession>? UserSessions { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Stories>? Stories { get; set; }
        public virtual ICollection<UserSubscription>? Subscriptions { get; set; }
        public virtual ICollection<UserSubscription>? Subscribers { get; set; } 
        public virtual ICollection<PostLike>? LikedPosts { get; set; }
        public virtual ICollection<CommentLike>? LikedComments { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }
        public virtual ICollection<Notification>? Notificatied { get; set; }

        public bool IsActive { get; set; }
        public bool IsPrivate { get; set; }
        
        public string? PushToken { get; set; }
    }
}
