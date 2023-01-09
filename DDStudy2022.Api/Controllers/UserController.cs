using DDStudy2022.Api.Models.Notifications;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Api.Services;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;
        public UserController(UserService userService, AuthService authService, LinkGeneratorService linkGenerator)
        {
            _userService = userService;
            _authService = authService;

            linkGenerator.LinkAvatarGenerator = x => Url.ControllerAction<AttachmentController>(
                    nameof(AttachmentController.GetUserAvatar),
                    new { userId = x.Id });
        }

        [HttpGet]
        public async Task<IEnumerable<UserAvatarModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        public async Task<UserProfileModel> GetCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _userService.GetUserModel(userId);

        }

        [HttpGet]
        public async Task<UserProfileModel> GetUserProfile(Guid userId)
        {
            var requesterId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (requesterId == default)
                throw new IdClaimConversionException();


            return await _userService.GetUserModel(userId, requesterId);
        }

        [HttpGet]
        public async Task<List<UserAvatarModel>> SearchUsers(string nameTag, int skip = 0, int take = 10) => await _userService.SearchUsers(nameTag, skip, take);

        [HttpPost]
        public async Task ChangeCurrentUserPassword(PasswordChangeRequest request)
        {
            if (!request.Id.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                request.Id = userId;
            }

            await _userService.ChangeUserPassword(request);
        }

        [HttpPost]
        public async Task ModifyUserInfo(ModifyUserInfoRequest request)
        {
            if (!request.Id.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new IdClaimConversionException();

                request.Id = userId;
            }

            await _userService.ModifyUserInfo(request);
        }

        [HttpPost]
        public async Task SuspendCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _userService.SuspendUser(userId);
        }

        [HttpPost]
        public async Task MakeCurrentAccountPrivate()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _userService.ChangeAccountPrivacySetting(userId);
        }

        [HttpGet]
        public async Task<ICollection<SessionModel>> GetCurrentSessions()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _userService.GetUserSessionModels(userId);
        }

        [HttpPost]
        public async Task DeactivateSession(SessionDeactivationRequest request)
            => await _userService.DeactivateSession(request.RefreshToken);

        [HttpGet]
        public async Task<List<NotificationModel>> GetNotifications(int skip = 0, int take = 10)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await _userService.GetNotifications(userId, skip, take);
        }
    }
}
