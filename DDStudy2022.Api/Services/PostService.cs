using AutoMapper;
using DDStudy2022.Api.Configs;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.CompilerServices;

namespace DDStudy2022.Api.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        private Func<AttachmentModel, string?>? _linkContentGenerator;
        private Func<UserModel, string?>? _linkAvatarGenerator;
        public void SetLinkGenerator(Func<AttachmentModel, string?> linkContentGenerator, Func<UserModel, string?> linkAvatarGenerator)
        {
            _linkAvatarGenerator = linkAvatarGenerator;
            _linkContentGenerator = linkContentGenerator;
        }

        public PostService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }



        public async Task CreatePost(CreatePostModel model)
        {
            var dbModel = _mapper.Map<Post>(model);

            await _context.Posts.AddAsync(dbModel);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PostModel>> GetPosts(int skip, int take)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Content).AsNoTracking().Take(take).Skip(skip).ToListAsync();

            var res = posts.Select(post =>
                new PostModel
                {
                    Author = new UserAvatarModel(_mapper.Map<UserModel>(post.Author), post.Author.Avatar == null ? null : _linkAvatarGenerator),
                    Description = post.Description,
                    Id = post.Id,
                    Content = post.Content.Select(x => 
                        new AttachmentWithLinkModel(_mapper.Map<AttachmentModel>(x), _linkContentGenerator)).ToList()
                }).ToList();

            return res;
        }

        public async Task<AttachmentModel> GetPostContent(Guid postContentId)
        {
            var res = await _context.PostContent.FirstOrDefaultAsync(x => x.Id == postContentId);

            return _mapper.Map<AttachmentModel>(res);
        }

        public async Task<PostModel> GetPostById(Guid postId)
        {
            var post = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Content).AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                throw new Exception("post not found");

            var res = new PostModel
            {
                Author = new UserAvatarModel(_mapper.Map<UserModel>(post.Author), post.Author.Avatar == null ? null : _linkAvatarGenerator),
                Description = post.Description,
                Id = post.Id,
                Content = post.Content.Select(x =>
                    new AttachmentWithLinkModel(_mapper.Map<AttachmentModel>(x), _linkContentGenerator)).ToList()
            };

            return res;
        }

        public async Task<List<PostModel>> GetUserPosts(Guid userId, int skip, int take)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Content).AsNoTracking().Where(p => p.AuthorId == userId).Take(take).Skip(skip).ToListAsync();

            var res = posts.Select(post =>
                new PostModel
                {
                    Author = new UserAvatarModel(_mapper.Map<UserModel>(post.Author), post.Author.Avatar == null ? null : _linkAvatarGenerator),
                    Description = post.Description,
                    Id = post.Id,
                    Content = post.Content.Select(x =>
                        new AttachmentWithLinkModel(_mapper.Map<AttachmentModel>(x), _linkContentGenerator)).ToList()
                }).ToList();

            return res;
        }

        public async Task UnlistPost(Guid postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                throw new Exception("post not found");

            post.IsShown = false;
            
            await _context.SaveChangesAsync();
        }

        /*
        public async Task ModifyPost(Guid postId, ModifyPostModel model)
        {
            var post = await GetPostById(postId);

            post.Description = model.Description;
            post.IsModified = true;
            // post.Content = model.Content;

            await _context.SaveChangesAsync();
        }
        */

        public async Task<List<CommentModel>> GetPostComments(Guid postId, int skip, int take)
        {
            var comments = await _context.PostComments
                .Include(c => c.Author).ThenInclude(x => x.Avatar)
                .AsNoTracking().Take(take).Skip(skip).ToListAsync();

            var res = comments.Select(comment =>
                new CommentModel
                {
                    // Всё так и задуманно, потом я переделаю автора с Guid на UserAvatarModel, пока что я не могу побороть автомаппер
                    AuthorId = comment.AuthorId,
                    Content = comment.Content,
                    PublishDate = comment.PublishDate.UtcDateTime
                }).ToList();

            return res;
        }

        public async Task AddComment(AddCommentModel model)
        {
            var dbModel = _mapper.Map<PostComment>(model);

            await _context.PostComments.AddAsync(dbModel);
            await _context.SaveChangesAsync();
        }
    }
}
