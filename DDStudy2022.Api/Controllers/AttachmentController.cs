using AutoMapper.Configuration.Annotations;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly AttachmentService _attachmentService;
        private readonly PostService _postService;

        public AttachmentController(AttachmentService attachmentService, PostService postService)
        {
            _attachmentService = attachmentService;
            _postService = postService;
        }

        [HttpPost]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
            => await _attachmentService.UploadFiles(files);

        [HttpPost]
        [Authorize]
        public async Task CreatePost([FromForm] CreatePostModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                await _postService.CreatePost(userId, model);
            }
            else
                throw new Exception("user not found");
        }

        [HttpPost]
        [Authorize]
        public async Task DeletePost(Guid postId)
        {
            var userIdString = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            var authorId = await _postService.GetPostAuthorId(postId);

            if (Guid.TryParse(userIdString, out var userId) 
                && authorId == userId)
            {
                await _postService.UnlistPost(postId);
            }
            else
                throw new Exception("only author can delete his posts");
        }

        // Нужно придумать, как получить весь content поста и вывести его в модель
        [HttpPost]
        [Authorize]
        private async Task ModifyPost(Guid postId, ModifyPostModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            var authorId = await _postService.GetPostAuthorId(postId);

            if (Guid.TryParse(userIdString, out var userId)
                && authorId == userId)
            {
                await _postService.ModifyPost(postId, model);
            }
            else
                throw new Exception("only author can modify his posts");
        }

        [HttpGet]
        public async Task<List<PostModel>> ShowUserPosts(Guid userId)
            => await _postService.GetPostModels(userId);

        [HttpGet]
        public async Task<PostModel> ShowPost(Guid postId) 
            => await _postService.GetPostModel(postId);


        [HttpGet]
        public async Task<FileResult> ShowAttachment(Guid attachmentId)
        {
            var attachment = await _attachmentService.GetAttachmentById(attachmentId);

            return File(System.IO.File.ReadAllBytes(attachment.FilePath), attachment.MimeType);
        }

        [HttpPost]
        [Authorize]
        public async Task AddCommentToPost(AddCommentModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                await _postService.AddComment(userId, model);
            }
            else
                throw new Exception("user not found");
        }

        [HttpGet]
        public async Task<List<CommentModel>> ShowComments(Guid postId)
        {
            return await _postService.GetPostComments(postId);
        }
    }
}
