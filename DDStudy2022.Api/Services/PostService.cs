using AutoMapper;
using DDStudy2022.Api.Configs;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Sessions;
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
        private readonly AttachmentService _attachmentService;
        private readonly UserService _userService;


        public PostService(IMapper mapper, DataContext context, AttachmentService attachmentService, UserService userService)
        {
            _mapper = mapper;
            _context = context;
            _attachmentService = attachmentService;
            _userService = userService;
        }



        public async Task CreatePost(Guid userId, CreatePostModel model)
        {
            var user = await _userService.GetUserWithPosts(userId);
            var postFiles = new List<PostAttachment>();
            
            var destFolder = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
            if (!string.IsNullOrEmpty(destFolder) && !Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            foreach (var attachment in model.Attachments)
            {
                var meta = await _attachmentService.UploadFile(attachment);
                
                var tempFile = _attachmentService.GetTempFileInfo(meta);
                var destFile = Path.Combine(destFolder, meta.TempId.ToString());
                System.IO.File.Copy(tempFile.FullName, destFile, true);

                postFiles.Add(new PostAttachment
                {
                    Author = user,
                    FilePath = destFile,
                    MimeType = meta.MimeType,
                    Name = meta.Name,
                    Size = meta.Size,
                });
            }

            user.Posts!.Add(new Post
            {
                Content = postFiles,
                Description = model.Description ?? "",
                PublishDate = DateTime.UtcNow,
            });

            await _context.SaveChangesAsync();
        }

        public async Task<List<PostModel>> GetPostModels(Guid userId)
        {
            var user = await _userService.GetUserWithPosts(userId);

            var posts = new List<PostModel>();
            foreach (var item in user.Posts!)
            {
                posts.Add(await GetPostModel(item.Id));
            }

            return posts;
        }

        public async Task<PostModel> GetPostModel(Guid postId)
        {
            var post = await GetPostWithContents(postId);

            return _mapper.Map<PostModel>(post);
        }

        private async Task<Post> GetPostById(Guid postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new Exception("Post doesn\'t exist");

            return post;
        }

        public async Task<Guid> GetPostAuthorId(Guid postId)
        {
            var post = await _context.Posts.Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new Exception("Post doesn\'t exist");

            return post.Author.Id;
        }

        private async Task<Post> GetPostWithContents(Guid postId)
        {
            var post = await _context.Posts.Include(p => p.Content).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new Exception("Post doesn\'t exist");

            return post;
        }

        private async Task<Post> GetPostWithComments(Guid postId)
        {
            // Заменить на ProjectTo
            var post = await _context.Posts.Include(p => p.Author)
                                           .Include(p => p.Comments)
                                           .ThenInclude(p => p.Author)
                                           .FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new Exception("Post doesn\'t exist");
            
            return post;
        }

        public async Task UnlistPost(Guid postId)
        {
            var post = await GetPostById(postId);
            post.IsShown = false;
            
            await _context.SaveChangesAsync();
        }

        public async Task ModifyPost(Guid postId, ModifyPostModel model)
        {
            var post = await GetPostById(postId);

            post.Description = model.Description;
            post.IsModified = true;
            // post.Content = model.Content;

            await _context.SaveChangesAsync();
        }



        public async Task<List<CommentModel>> GetPostComments(Guid postId)
        {
            var post = await GetPostWithComments(postId);

            return _mapper.Map<List<CommentModel>>(post.Comments);
        }

        public async Task AddComment(Guid userId, AddCommentModel model)
        {
            var post = await GetPostWithComments(model.PostId);
            var user = await _userService.GetUserWithPosts(userId);

            post.Comments!.Add(new PostComment
            {
                Author = user,
                Content = model.Content,
                PublishDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
    }
}
