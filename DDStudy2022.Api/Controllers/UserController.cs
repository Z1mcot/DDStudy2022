using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Stories;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Api.Services;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Enums;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly StoriesService _storiesService;
        private readonly SubscriptionService _subscriptionService;
        
        public UserController(
            UserService userService, 
            PostService postService, 
            StoriesService storiesService,
            SubscriptionService subscriptionService,
            LinkGeneratorService linkGenerator
            )
        {
            _userService = userService;
            _postService = postService;
            _storiesService = storiesService;
            _subscriptionService = subscriptionService;

            linkGenerator.LinkAvatarGenerator = x => Url.ControllerAction<AttachmentController>(
                    nameof(AttachmentController.GetUserAvatar),
                    new { userId = x.Id });
        }

        [HttpGet]
        public async Task<IEnumerable<UserAvatarModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        [Route("current")]
        public async Task<UserProfileModel> GetCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _userService.GetUserModel(userId);

        }

        [HttpGet]
        [Route("{userId:guid}")]
        public async Task<UserProfileModel> GetUserProfile(Guid userId)
        {
            var requesterId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (requesterId == default)
                throw new IdClaimConversionException();


            return await _userService.GetUserModel(userId, requesterId);
        }

        [HttpGet]
        [Route("search")]
        public async Task<List<UserAvatarModel>> SearchUsers(string nameTag, int skip = 0, int take = 10) 
            => await _userService.SearchUsers(nameTag, skip, take);

        [HttpPut]
        public async Task ModifyUserInfo(ModifyUserInfoRequest request)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _userService.ModifyUserInfo(userId, request);
        }

        [HttpPost]
        [Route("suspend")]
        public async Task SuspendCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _userService.SuspendUser(userId);
        }

        [HttpPost]
        [Route("change-privacy")]
        public async Task MakeCurrentAccountPrivate()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _userService.ChangeAccountPrivacySetting(userId);
        }

        [HttpGet]
        [Route("sessions")]
        public async Task<ICollection<SessionModel>> GetCurrentSessions()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _userService.GetUserSessionModels(userId);
        }

        [HttpPost]
        [Route("sessions/{sessionId:guid}")]
        public async Task DeactivateSession(Guid sessionId)
            => await _userService.DeactivateSession(sessionId);
        
        [HttpGet]
        [Route("{authorId:guid}/posts")]
        public async Task<List<PostModel>> ShowUserPosts(Guid authorId, int skip = 0, int take = 10)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _postService.GetUserPosts(userId, authorId, skip, take);
        }
        
        [HttpPost]
        [Route("{subscribeToUserId:guid}/subscribe")]
        public async Task SubscribeToUser(Guid subscribeToUserId)
        {
            if (subscribeToUserId == default)
                throw new Exception();

            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _subscriptionService.SubscribeToUser(userId, subscribeToUserId);
        }

        [HttpPost]
        [Route("{unsubscribeFromUserId:guid}/unsubscribe")]
        public async Task UnsubscribeFromUser(Guid unsubscribeFromUserId)
        {
            if (unsubscribeFromUserId == default)
                throw new Exception();
            
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _subscriptionService.UnsubscribeFromUser(userId, unsubscribeFromUserId);
        }

        [HttpGet]
        [Route("{userId:guid}/subscribers")]
        public async Task<ICollection<UserAvatarModel>> GetUserSubscribers
            (Guid userId, SubscriptionStatus status = SubscriptionStatus.Active,  int skip = 0, int take = 10)
        {
            if (status == SubscriptionStatus.Pending && userId != User.GetClaimValue<Guid>(ClaimNames.Id))
                throw new PrivateAccountSubscribersException();
            
            return await _subscriptionService.GetSubscribers(userId, status, skip, take);
        }

        [HttpGet]
        [Route("{userId:guid}/subscriptions")]
        public async Task<ICollection<UserAvatarModel>> GetUserSubscriptions
            (Guid userId, SubscriptionStatus status = SubscriptionStatus.Active, int skip = 0, int take = 10)
        {
            if (status == SubscriptionStatus.Pending && userId != User.GetClaimValue<Guid>(ClaimNames.Id))
                throw new PrivateAccountSubscriptionException();
            
            return await _subscriptionService.GetSubscriptions(userId, status, skip, take);
        }

        [HttpPost]
        [Route("subscriptions/{userId:guid}/approve")]
        public async Task ConfirmSubRequest(Guid userId)
        {
            var authorId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (authorId == default)
                throw new IdClaimConversionException();
            
            await _subscriptionService.ConfirmSubscriber(authorId, userId);
        }
        
        [HttpGet]
        [Route("{authorId:guid}/stories")]
        public async Task<List<StoriesModel>> GetUserStories(Guid authorId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _storiesService.GetUserStories(userId, authorId);
        }
    }
}
