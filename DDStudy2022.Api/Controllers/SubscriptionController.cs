using DDStudy2022.Api.Models.Subscriptions;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Api.Services;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionService _subscriptionService;

        public SubscriptionController(SubscriptionService subscriptionService, LinkGeneratorService linkGenerator)
        {
            _subscriptionService = subscriptionService;


            linkGenerator.LinkAvatarGenerator = x => Url.ControllerAction<AttachmentController>(
                    nameof(AttachmentController.GetUserAvatar),
                    new { userId = x.Id });
        }

        [HttpPost]
        public async Task SubscribeToUser(SubscribtionRequest request)
        {
            if (!request.SubscriberId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                request.SubscriberId = userId;
            }

            await _subscriptionService.SubscribeToUser(request);
        }

        [HttpGet]
        public async Task<ICollection<UserAvatarModel>> GetUserSubscribers(Guid userId)
        {
            return await _subscriptionService.GetSubscribers(userId);
        }

        [HttpGet]
        public async Task<ICollection<UserAvatarModel>> GetUserSubscriptions(Guid userId)
        {
            return await _subscriptionService.GetSubscriptions(userId);
        }

        //[HttpPost]
        //public async Task UnsubscribeFromUser( request)
        //{
        //    if (!request.SubscriberId.HasValue)
        //    {
        //        var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
        //        if (userId == default)
        //            throw new IdClaimConversionException();

        //        request.SubscriberId = userId;
        //    }
        //    await _subscriptionService.Unsubscribe(request);
        //}

        [HttpGet]
        public async Task<ICollection<UserAvatarModel>> GetSubRequests()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _subscriptionService.GetSubscriptionRequests(userId);
        }

        [HttpPost]
        public async Task ConfirmSubRequest(Guid userId)
        {
            var authorId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (authorId == default)
                throw new IdClaimConversionException();
            await _subscriptionService.ConfirmSubscriber(authorId, userId);
        }
    }
}
