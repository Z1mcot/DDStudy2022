using DDStudy2022.Api.Models.Stories;
using DDStudy2022.Api.Services;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class StoriesController : ControllerBase
    {
        private readonly StoriesService _storiesService;
        public StoriesController(StoriesService storiesService, LinkGeneratorService linkGenerator)
        {
            _storiesService = storiesService;

            linkGenerator.LinkAvatarGenerator = x => Url.ControllerAction<AttachmentController>(
                    nameof(AttachmentController.GetUserAvatar),
                    new { userId = x.Id }
                    );
            linkGenerator.LinkStoriesGenerator = x => Url.ControllerAction<AttachmentController>(
                    nameof(AttachmentController.GetStoryContent),
                    new { storyContentId = x.Id }
                    );
        }

        [HttpPost]
        public async Task CreateStory(CreateStoriesRequest request)
        {
            if (!request.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                request.AuthorId = userId;
            }

            await _storiesService.AddStoriesToUser(request);
        }

        [HttpPost]
        public async Task DeleteStory(Guid storyId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _storiesService.UnlistStory(userId, storyId);
        }

        [HttpGet]
        public async Task<List<StoriesModel>> GetStories()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _storiesService.GetStoriesFromSubscriptions(userId);
        }

        [HttpGet]
        public async Task<List<StoriesModel>> GetUserStories(Guid authorId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _storiesService.GetUserStories(userId, authorId);
        }

        [HttpGet]
        public async Task<StoriesModel> GetStoryById(Guid storyId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _storiesService.GetStoryById(userId, storyId);
        }
    }
}
