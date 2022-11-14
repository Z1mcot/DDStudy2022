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
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.CompilerServices;

namespace DDStudy2022.Api.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        private Func<PostAttachment, string?>? _linkContentGenerator;
        private Func<User, string?>? _linkAvatarGenerator;
        public void SetLinkGenerator(Func<PostAttachment, string?> linkContentGenerator, Func<User, string?> linkAvatarGenerator)
        {
            _linkAvatarGenerator = linkAvatarGenerator;
            _linkContentGenerator = linkContentGenerator;
        }

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
                throw new Exception("post not found");
            if (post.AuthorId != request.AuthorId)
                throw new Exception("You are not authorized for this action");


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

        public async Task<List<PostModel>> GetPosts(int skip, int take)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Content).AsNoTracking().Take(take).Skip(skip).ToListAsync();

            var res = posts.Select(post =>
                new PostModel
                {
                    Author = _mapper.Map<User, UserAvatarModel>(post.Author, opt => opt.AfterMap(FixAvatar)),
                    Description = post.Description,
                    Id = post.Id,
                    Content = post.Content.Select(x =>
                        _mapper.Map<PostAttachment, AttachmentExternalModel>(x, opt => opt.AfterMap(FixContent))).ToList()
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
                Author = _mapper.Map<User, UserAvatarModel>(post.Author, opt => opt.AfterMap(FixAvatar)),
                Description = post.Description,
                Id = post.Id,
                Content = post.Content.Select(x =>
                    _mapper.Map<PostAttachment, AttachmentExternalModel>(x, opt => opt.AfterMap(FixContent))).ToList()
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
                    Author = _mapper.Map<User, UserAvatarModel>(post.Author, opt => opt.AfterMap(FixAvatar)),
                    Description = post.Description,
                    Id = post.Id,
                    Content = post.Content.Select(x =>
                        _mapper.Map<PostAttachment, AttachmentExternalModel>(x, opt => opt.AfterMap(FixContent))).ToList()
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





        public async Task<List<CommentModel>> GetPostComments(Guid postId, int skip, int take)
        {
            var comments = await _context.PostComments.
                Include(c => c.Author).ThenInclude(u => u.Avatar)
                .AsNoTracking().Where(p => p.Id == postId).ToListAsync();


            var res = comments.Select(comment =>
                new CommentModel
                {
                    Author = _mapper.Map<User, UserAvatarModel>(comment.Author, opt => opt.AfterMap(FixAvatar)),
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

        private void FixAvatar(User src, UserAvatarModel dest)
        {
            dest.AvatarLink = src.Avatar == null ? null : _linkAvatarGenerator?.Invoke(src);
        }
        private void FixContent(PostAttachment src, AttachmentExternalModel dest)
        {
            dest.ContentLink = _linkContentGenerator?.Invoke(src);
        }
    }
}
