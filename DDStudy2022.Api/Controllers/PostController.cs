using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Extensions;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.Api.Models.Likes;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/posts")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly CommentService _commentService;
        public PostController(PostService postService, CommentService commentService, LinkGeneratorService linkGenerator)
        {
            _postService = postService;
            _commentService = commentService;

            linkGenerator.LinkAvatarGenerator = x => Url.ControllerAction<AttachmentController>(
                    nameof(AttachmentController.GetUserAvatar),
                    new { userId = x.Id }
                    );
            linkGenerator.LinkContentGenerator = x => Url.ControllerAction<AttachmentController>(
                    nameof(AttachmentController.GetPostContent), 
                    new { postContentId = x.Id }
                    );
        }

        [HttpPost]
        public async Task CreatePost(CreatePostRequest request)
        {
            if (!request.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                request.AuthorId = userId;
            } 

            await _postService.CreatePost(request);
        }

        [HttpDelete]
        public async Task DeletePost(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException(); 
            
            await _postService.UnlistPost(userId, postId);
        }

        // Пока он только добавляет новые картинки к посту
        [HttpPut]
        public async Task ModifyPost(Guid postId, ModifyPostRequest request)
        {
            if (!request.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                request.AuthorId = userId;
            }

            await _postService.ModifyPost(postId, request);
        }

        [HttpGet]
        [Route("{postId:guid}")]
        public async Task<PostModel> ShowPost(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _postService.GetPostById(userId, postId);
        }

        [HttpGet]
        public async Task<List<PostModel>> GetPosts(int skip = 0, int take = 10)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _postService.GetPosts(userId, skip, take);
        }

        [HttpPost]
        [Route("{postId:guid}/comments")]
        public async Task AddCommentToPost(Guid postId, AddCommentModel model)
        {
            var authorId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (authorId == default)
                throw new IdClaimConversionException();

            await _commentService.AddComment(authorId, postId, model);
        }

        [HttpPut]
        [Route("comments/{commentId:guid}")]
        public async Task ModifyPostComment(Guid commentId, ModifyCommentModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _commentService.ModifyComment(model, userId, commentId);
        }

        [HttpDelete]
        [Route("comments/{commentId:guid}")]
        public async Task DeleteComment(Guid commentId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _commentService.RemoveComment(userId, commentId);
        }

        [HttpGet]
        [Route("{postId:guid}/comments")]
        public async Task<List<CommentModel>> ShowComments(Guid postId, int skip = 0, int take = 10)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _commentService.GetPostComments(userId, postId, skip, take);
        }

        [HttpPost]
        [Route("{postId:guid}/like")]
        public async Task LikePost(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _postService.LikePost(userId, postId);
        }

        [HttpPost]
        [Route("comments/{commentId:guid}/like")]
        public async Task LikeComment(Guid commentId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _commentService.LikeComment(userId, commentId);
        }
    }
}
