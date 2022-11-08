using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Extensions;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.DAL.Entities;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        public PostController(PostService postService)
        {
            _postService = postService;
            _postService.SetLinkGenerator(
                linkAvatarGenerator: x =>
                Url.Action(nameof(UserController.GetUserAvatar), "User", new
                {
                    userId = x.Id,
                    download = false
                }),
                linkContentGenerator: x => Url.Action(nameof(GetPostContent), new
                {
                    postContentId = x.Id,
                    download = false
                }));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetPostContent(Guid postContentId, bool download = false)
        {
            var attach = await _postService.GetPostContent(postContentId);
            var fs = new FileStream(attach.FilePath, FileMode.Open);
            if (download)
                return File(fs, attach.MimeType, attach.Name);
            else
                return File(fs, attach.MimeType);

        }

        [HttpPost]
        public async Task CreatePost(CreatePostRequest request)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new Exception("not authorize");

            var model = new CreatePostModel
            {
                AuthorId = userId,
                Description = request.Description,
                Content = request.Content.Select(x =>
                new MetadataWithPathModel(x, q => Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Attachments",
                    q.TempId.ToString()), userId)).ToList()
            };

            model.Content.ForEach(x =>
            {
                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), x.TempId.ToString()));
                if (tempFi.Exists)
                {
                    var destFi = new FileInfo(x.FilePath);
                    if (destFi.Directory != null && !destFi.Directory.Exists)
                        destFi.Directory.Create();

                    System.IO.File.Copy(tempFi.FullName, x.FilePath, true);
                    tempFi.Delete();
                }
            });

            await _postService.CreatePost(model);
        }

        [HttpPost]
        public async Task DeletePost(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            var postAuthor = (await _postService.GetPostById(postId)).Author.Id;
            if (userId == default || userId != postAuthor)
                throw new Exception("not authorized");
            else
                await _postService.UnlistPost(postId);
        }

        /*
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
        */

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<PostModel>> ShowUserPosts(Guid userId, int skip = 0, int take = 10)
            => await _postService.GetUserPosts(userId, skip, take);

        [HttpGet]
        [AllowAnonymous]
        public async Task<PostModel> ShowPost(Guid postId)
            => await _postService.GetPostById(postId);

        [HttpGet]
        public async Task<List<PostModel>> GetPosts(int skip = 0, int take = 10) 
            => await _postService.GetPosts(skip, take);

        [HttpPost]
        public async Task AddCommentToPost(AddCommentRequest request)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new Exception("user not found");

            var model = new AddCommentModel
            {
                AuthorId = userId,
                Content = request.Content
            };

            await _postService.AddComment( model);
            
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<CommentModel>> ShowComments(Guid postId, int skip = 0, int take = 10)
        {
            return await _postService.GetPostComments(postId, skip, take);
        }
    }
}
