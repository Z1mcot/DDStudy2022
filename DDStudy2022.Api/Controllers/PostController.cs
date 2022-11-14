using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Extensions;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

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
                Url.ControllerAction<AttachmentController>(
                    nameof(AttachmentController.GetUserAvatar),
                    arg: new { userId = x.Id }),
                linkContentGenerator: x => 
                Url.ControllerAction<AttachmentController>(
                    nameof(AttachmentController.GetPostContent), 
                    arg: new { postContentId = x.Id }));
        }

        

        [HttpPost]
        public async Task CreatePost(CreatePostRequest request)
        {
            if (!request.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new Exception("not authorized");

                request.AuthorId = userId;
            } 

            await _postService.CreatePost(request);
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

        // Пока он только добавляет новые картинки к посту
        [HttpPost]
        public async Task ModifyPost(Guid postId, ModifyPostRequest request)
        {
            if (!request.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new Exception("not authorized");

                request.AuthorId = userId;
            }

            await _postService.ModifyPost(postId, request);
        }


        [HttpGet]
        public async Task<List<PostModel>> ShowUserPosts(Guid userId, int skip = 0, int take = 10)
            => await _postService.GetUserPosts(userId, skip, take);

        [HttpGet]
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
                PostId = request.PostId,
                AuthorId = userId,
                Content = request.Content
            };

            await _postService.AddComment(model);
            
        }

        [HttpGet]
        public async Task<List<CommentModel>> ShowComments(Guid postId, int skip = 0, int take = 10)
        {
            return await _postService.GetPostComments(postId, skip, take);
        }
    }
}
