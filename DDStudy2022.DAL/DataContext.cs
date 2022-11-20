using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<Avatar>()
                .ToTable(nameof(Avatars));
            modelBuilder.Entity<PostAttachment>()
                .ToTable(nameof(PostAttachment));

            modelBuilder.Entity<UserSubscription>()
                .HasKey(us => new { us.AuthorId, us.SubscriberId });

            modelBuilder.Entity<UserSubscription>()
                .HasOne(us => us.Author)
                .WithMany(u => u.Subscribers)
                .HasForeignKey(us => us.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // для предотвращения циклов и проблем с multiple cascade path
                                                    // , но придётся вручную удалять подпивчеков
            modelBuilder.Entity<UserSubscription>()
                .HasOne(us => us.Subscriber)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(us => us.SubscriberId);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(b => b.MigrationsAssembly("DDStudy2022.Api"));

        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostAttachment> PostContent => Set<PostAttachment>();
        public DbSet<PostComment> PostComments => Set<PostComment>();
        public DbSet<UserSubscription> Subscriptions => Set<UserSubscription>();
    }
}
