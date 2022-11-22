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
    [Route("api/[controller]/[action]")]
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

        [HttpPost]
        public async Task DeletePost(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();
            else
                await _postService.UnlistPost(userId, postId);
        }

        // Пока он только добавляет новые картинки к посту
        [HttpPost]
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
        public async Task<List<PostModel>> ShowUserPosts(Guid authorId, int skip = 0, int take = 10)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _postService.GetUserPosts(userId, authorId, skip, take);
        }

        [HttpGet]
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
        public async Task AddCommentToPost(AddCommentModel model)
        {
            if (!model.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                model.AuthorId = userId;
            }

            await _commentService.AddComment(model);
        }

        [HttpPost]
        public async Task ModifyPostComment(ModifyCommentModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _commentService.ModifyComment(model, userId);
        }

        [HttpPost]
        public async Task DeleteComment(Guid commentId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _commentService.RemoveComment(userId, commentId);
        }

        [HttpGet]
        public async Task<List<CommentModel>> ShowComments(Guid postId, int skip = 0, int take = 10)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _commentService.GetPostComments(userId, postId, skip, take);
        }

        [HttpPost]
        public async Task AddLikeToPost(ModifyPostLikeModel model)
        {
            if (!model.UserId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                model.UserId = userId;
            }

            await _postService.AddLikeToPost(model);
        }

        [HttpPost]
        public async Task RemoveLikeFromPost(ModifyPostLikeModel model)
        {
            if (!model.UserId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                model.UserId = userId;
            }

            await _postService.RemoveLikeFromPost(model);
        }

        [HttpPost]
        public async Task AddLikeToComment(ModifyCommentLikeModel model)
        {
            if (!model.UserId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                model.UserId = userId;
            }

            await _commentService.AddLikeToComment(model);
        }

        [HttpPost]
        public async Task RemoveLikeFromComments(ModifyCommentLikeModel model)
        {
            if (!model.UserId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                model.UserId = userId;
            }

            await _commentService.RemoveLikeFromComment(model);
        }

    }
}
