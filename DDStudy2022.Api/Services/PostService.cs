using AutoMapper;
using DDStudy2022.Api.Configs;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace DDStudy2022.Api.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly AttachmentService _attachmentService;


        public PostService(IMapper mapper, DataContext context, AttachmentService attachmentService)
        {
            _mapper = mapper;
            _context = context;
            _attachmentService = attachmentService;
        }

        private async Task<DAL.Entities.User> GetUser(Guid userId)
        {
            var user = await _context.Users.Include(u => u.Posts).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new Exception("user not found");
            return user;
        }

        public async Task CreatePost(Guid userId, CreatePostModel model)
        {
            var user = await GetUser(userId);
            var postFiles = new List<PostImage>();
            
            var destFolder = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
            if (!string.IsNullOrEmpty(destFolder) && !Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            foreach (var attachment in model.Attachments)
            {
                var meta = await _attachmentService.UploadFile(attachment);
                
                var tempFile = _attachmentService.GetTempFileInfo(meta);
                var destFile = Path.Combine(destFolder, meta.TempId.ToString());
                System.IO.File.Copy(tempFile.FullName, destFile, true);

                postFiles.Add(new PostImage
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

        public async Task<List<PostModel>> GetUserPosts(Guid userId)
        {
            var user = await GetUser(userId);

            var posts = new List<PostModel>();
            foreach (var item in user.Posts!)
            {
                posts.Add(await GetPost(item.Id));
            }

            return posts;
        }

        public async Task<PostModel> GetPost(long postId)
        {
            var post = await GetPostWithContents(postId);

            return _mapper.Map<PostModel>(post);
        }

        private async Task<Post> GetPostWithContents(long postId)
        {
            var post = await _context.Posts.Include(p => p.Content).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new Exception("Post doesn\'t exist");
            return post;
        }

        private async Task<Post> GetPostWithComments(long postId)
        {
            var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new Exception("Post doesn\'t exist");
            
            return post;
        }

        public async Task<AttachmentModel> GetAttachmentById(long attachmentId)
        {
            var attachment = await _context.Attachments.FirstOrDefaultAsync(p => p.Id == attachmentId);

            return _mapper.Map<AttachmentModel>(attachment);
        }

        public async Task<List<CommentModel>> GetPostComments(long postId)
        {
            var post = await GetPostWithComments(postId);

            return _mapper.Map<List<CommentModel>>(post.Comments);
        }

        public async Task AddComment(Guid userId, AddCommentModel model)
        {
            var post = await GetPostWithComments(model.PostId);
            var user = await GetUser(userId);

            post.Comments!.Add(new PostComment
            {
                Author = user,
                Content = model.Content,
                Post = post
            });

            await _context.SaveChangesAsync();
        }
    }
}
