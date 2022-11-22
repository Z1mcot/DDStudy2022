using AutoMapper;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Likes;
using DDStudy2022.Api.Models.Posts;
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

        public PostService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
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
        }

        public async Task ModifyPost(Guid postId, ModifyPostRequest request)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                throw new PostNotFoundException();
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

            post.Description = model.Description;
            post.IsModified = true;
            post.Content = _mapper.Map<List<PostAttachment>>(model.Content);

            await _context.SaveChangesAsync();
        }

        public async Task UnlistPost(Guid userId, Guid postId)
        {
            var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                throw new PostNotFoundException();
            if (post.AuthorId != userId)
                throw new ModifyPostException();

            post.IsShown = false;

            await _context.SaveChangesAsync();
        }

        // К сожалению сюда IsAuthorizedToSeePosts не впихнуть.
        public async Task<List<PostModel>> GetPosts(Guid userId, int skip, int take)
        {
            var dbPosts = await _context.Posts
                .IncludeAuthorWithAvatar()
                .IncludeAuthorWithSubscribers()
                .Include(x => x.Content)
                .Include(x => x.Likes)
                .AsNoTracking()
                .Where(p => p.AuthorId != userId // нет смысла выводит здесь свои же посты
                            && p.Author.IsActive // Проще было бы конечно удалять пользователей, но раз уж сделал так, то приходиться проверять рабочий ли юзер
                            && p.IsShown // Аналогично с постами
                            && (!p.Author.IsPrivate || p.Author.Subscribers!.Any(u => u.SubscriberId == userId && u.IsConfirmed))
                            ) 
                .OrderByDescending(x => x.PublishDate).Skip(skip).Take(take)
                .ToListAsync();

            var posts = new List<PostModel>();
            foreach (var post in dbPosts)
            {
                // Хорошо было бы оставить Select, но тогда мы бы не смогли сделать отображение того лайкнули ли мы пост или нет
                var model = _mapper.Map<Post, PostModel>(post, opt =>
                {
                    opt.AfterMap((src, dest)
                        => dest.IsLiked = src.Likes!.Any(s => s.UserId == userId && s.PostId == post.Id));
                });
                posts.Add(model);
            }

            return posts;
        }

        public async Task<AttachmentModel> GetPostContent(Guid userId, Guid postContentId)
        {
            var res = await _context.PostContent
                .Include(x => x.Post)
                .FirstOrDefaultAsync(x => x.Id == postContentId && x.Post.IsShown);
            
            if (res == null)
                throw new AttachmentNotFoundException();
            if (!await IsAuthorizedToSeePosts(userId, res.AuthorId))
                throw new PrivateAccountNonsubException();


            return _mapper.Map<AttachmentModel>(res);
        }

        public async Task<PostModel> GetPostById(Guid userId, Guid postId)
        {
            var post = await _context.Posts
                .IncludeAuthorWithAvatar()
                .Include(x => x.Content)
                .Include(x => x.Likes)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null || !post.IsShown)
                throw new PostNotFoundException();
            if (!await IsAuthorizedToSeePosts(userId, post.AuthorId))
                throw new PrivateAccountNonsubException();

            var postModel = _mapper.Map<Post, PostModel>(post, opt => 
            {
                opt.AfterMap((src, dest) 
                    => dest.IsLiked = src.Likes!.Any(s => s.UserId == userId && s.PostId == postId)); 
            });
            return postModel;
        }

        public async Task<List<PostModel>> GetUserPosts(Guid userId, Guid authorId, int skip, int take)
        {
            var isSubbed = await IsAuthorizedToSeePosts(userId, authorId);
            if (!isSubbed)
                throw new PrivateAccountNonsubException();

            var dbPosts = await _context.Posts
                .IncludeAuthorWithAvatar()
                .Include(x => x.Content)
                .Include(x => x.Likes)
                .AsNoTracking()
                .Where(p => p.AuthorId == authorId && p.IsShown)
                .OrderByDescending(x => x.PublishDate).Skip(skip).Take(take)
                .ToListAsync();

            var posts = new List<PostModel>();
            foreach (var post in dbPosts)
            {
                // Хорошо было бы оставить Select, но тогда мы бы не смогли сделать отображение того лайкнули ли мы пост или нет
                var model = _mapper.Map<Post, PostModel>(post, opt =>
                {
                    opt.AfterMap((src, dest)
                        => dest.IsLiked = src.Likes!.Any(s => s.UserId == userId && s.PostId == post.Id));
                });
                posts.Add(model);
            }

            return posts;
        }

        public async Task AddLikeToPost(ModifyPostLikeModel model)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == model.PostId);

            if (post == null || !post.IsShown)
                throw new PostNotFoundException();
            if (!await IsAuthorizedToSeePosts((Guid)model.UserId!, post.AuthorId))
                throw new PrivateAccountNonsubException();

            var dbLike = _mapper.Map<PostLike>(model);

            await _context.PostLikes.AddAsync(dbLike);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveLikeFromPost(ModifyPostLikeModel model)
        {
            var like = await _context.PostLikes.FirstOrDefaultAsync(l => l.UserId == model.UserId && l.PostId == model.PostId);
            if (like == null)
                throw new LikeNotFoundException();

            _context.PostLikes.Remove(like);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> IsAuthorizedToSeePosts(Guid userId, Guid authorId)
        {
            if (userId == authorId) 
                return true;

            var dbAuthor = await _context.Users.Include(u => u.Subscribers).FirstAsync(u => u.Id == authorId);
            if (!dbAuthor.IsActive)
                return false;

            if (!dbAuthor.IsPrivate || dbAuthor.Subscribers!.Any(s => s.SubscriberId == userId && s.IsConfirmed))
                return true;
            return false;
        }
    }
}
