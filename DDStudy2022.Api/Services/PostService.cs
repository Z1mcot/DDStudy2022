using AutoMapper;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Likes;
using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Common.Enums;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.Common.Extensions;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDStudy2022.Api.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly NotifyService _notifyService;

        public PostService(IMapper mapper, DataContext context, NotifyService notifyService)
        {
            _mapper = mapper;
            _context = context;
            _notifyService = notifyService;
        }

        public async Task CreatePost(CreatePostRequest request)
        {
            var model = _mapper.Map<CreatePostModel>(request);

            model.Content.ForEach(x =>
            {
                x.AuthorId = model.AuthorId;
                x.FilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Attachments",
                    x.TempId.ToString());
           
                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), x.TempId.ToString()));
                if (tempFi.Exists)
                {
                    var destFi = new FileInfo(x.FilePath);
                    if (destFi.Directory != null && !destFi.Directory.Exists)
                        destFi.Directory.Create();

                    File.Move(tempFi.FullName, x.FilePath, true);
                }
            });

            var dbModel = _mapper.Map<Post>(model);

            await _context.Posts.AddAsync(dbModel);
            await _context.SaveChangesAsync();

            await _notifyService.SendNewPostNotification(dbModel);
        }

        public async Task ModifyPost(Guid postId, ModifyPostRequest request)
        {
            var post = await _context.Posts.Include(x => x.Content).FirstOrDefaultAsync(x => x.Id == postId && x.IsShown) 
                ?? throw new PostNotFoundException();
            
            if (post.AuthorId != request.AuthorId)
                throw new ModifyPostException();


            var model = _mapper.Map<ModifyPostModel>(request);
            model.Content.ForEach(x =>
            {
                x.AuthorId = model.AuthorId;
                x.FilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Attachments",
                    x.TempId.ToString());

                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), x.TempId.ToString()));
                if (tempFi.Exists)
                {
                    var destFi = new FileInfo(x.FilePath);
                    if (destFi.Directory != null && !destFi.Directory.Exists)
                        destFi.Directory.Create();

                    File.Move(tempFi.FullName, x.FilePath, true);
                }
            });

            var content = _mapper.Map<List<PostAttachment>>(model.Content);
            _context.RemoveRange(post.Content.Except(content));

            post.Description = model.Description;
            post.IsModified = true;
            post.Content = content;

            await _context.SaveChangesAsync();
        }

        public async Task UnlistPost(Guid userId, Guid postId)
        {
            var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId && x.IsShown) 
                ?? throw new PostNotFoundException();

            if (post.AuthorId != userId)
                throw new ModifyPostException();

            post.IsShown = false;

            await _context.SaveChangesAsync();
        }

        public async Task<List<PostModel>> GetPosts(Guid userId, int skip, int take)
        {
            var dbPosts = await _context.Posts
                .Include(s => s.Author).ThenInclude(s => s.Avatar)
                .Include(s => s.Author).ThenInclude(s => s.Subscribers)
                .Include(x => x.Content)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .AsNoTracking()
                .Where(p => p.AuthorId != userId // нет смысла выводит здесь свои же посты
                            && p.Author.IsActive
                            && p.IsShown
                            && (!p.Author.IsPrivate || p.Author.Subscribers!.Any(u => u.SubscriberId == userId && u.Status == SubscriptionStatus.Active))
                            ) 
                .OrderByDescending(x => x.PublishDate).Skip(skip).Take(take)
                .ToListAsync();

            return dbPosts.Select(post => _mapper.Map<Post, PostModel>(post, opt =>
            {
                opt.AfterMap((src, dest) => dest.IsLiked = src.Likes != null && src.Likes.Any(s => s.UserId == userId && s.PostId == post.Id) ? 1 : 0);
            })).ToList();
        }

        public async Task<AttachmentModel> GetPostContent(Guid userId, Guid postContentId)
        {
            var res = await _context.PostContent
                .Include(x => x.Post)
                .FirstOrDefaultAsync(x => x.Id == postContentId && x.Post.IsShown)
                ?? throw new AttachmentNotFoundException();
            
            if (!await IsAuthorizedToSeePosts(userId, res.AuthorId))
                throw new PrivateAccountException();


            return _mapper.Map<AttachmentModel>(res);
        }

        public async Task<PostModel> GetPostById(Guid userId, Guid postId)
        {
            var post = await _context.Posts
                .Include(s => s.Author).ThenInclude(s => s.Avatar)
                .Include(x => x.Content)
                .Include(x => x.Likes)
                .Include(x => x.Comments)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post is not { IsShown: true })
                throw new PostNotFoundException();
            if (!await IsAuthorizedToSeePosts(userId, post.AuthorId))
                throw new PrivateAccountException();

            var postModel = _mapper.Map<Post, PostModel>(post, opt => 
            {
                opt.AfterMap((src, dest)
                    => dest.IsLiked = src.Likes != null && src.Likes.Any(s => s.UserId == userId && s.PostId == post.Id) ? 1 : 0);
            });
            return postModel;
        }

        public async Task<List<PostModel>> GetUserPosts(Guid userId, Guid authorId, int skip, int take)
        {
            var isSubbed = await IsAuthorizedToSeePosts(userId, authorId);
            if (!isSubbed)
                throw new PrivateAccountException();

            var dbPosts = await _context.Posts
                .Include(s => s.Author).ThenInclude(s => s.Avatar)
                .Include(x => x.Content)
                .Include(x => x.Likes)
                .Include(x => x.Comments)
                .AsNoTracking()
                .Where(p => p.AuthorId == authorId && p.IsShown)
                .OrderByDescending(x => x.PublishDate).Skip(skip).Take(take)
                .ToListAsync();

            return dbPosts.Select(post => _mapper.Map<Post, PostModel>(post, opt => { opt.AfterMap((src, dest) => dest.IsLiked = src.Likes != null && src.Likes.Any(s => s.UserId == userId && s.PostId == post.Id) ? 1 : 0); })).ToList();
        }

        public async Task LikePost(Guid userId, Guid postId)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null || !post.IsShown)
                throw new PostNotFoundException();
            if (!await IsAuthorizedToSeePosts(userId, post.AuthorId))
                throw new PrivateAccountException();
            
            var like = await _context.PostLikes.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
            if (like != null)
            {
                _context.PostLikes.Remove(like);
            }
            else
            {
                var likeModel = new PostLike
                {
                    UserId = userId,
                    PostId = postId
                };
                
                await _context.PostLikes.AddAsync(likeModel);
            }
            
            await _context.SaveChangesAsync();
        }

        private async Task<bool> IsAuthorizedToSeePosts(Guid userId, Guid authorId)
        {
            if (userId == authorId) 
                return true;

            var dbAuthor = await _context.Users.Include(u => u.Subscribers).FirstAsync(u => u.Id == authorId);
            if (!dbAuthor.IsActive)
                return false;

            return !dbAuthor.IsPrivate || dbAuthor.Subscribers!.Any(s => s.SubscriberId == userId && s.Status == SubscriptionStatus.Active);
        }
    }
}
