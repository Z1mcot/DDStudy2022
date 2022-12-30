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
            => await _userService.GetUserModel(userId);

        [HttpGet]
        public async Task<List<UserAvatarModel>> SearchUsers(string nameTag) => await _userService.SearchUsers(nameTag);

        [HttpPost]
        public async Task ChangeCurrentUserPassword(PasswordChangeModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _userService.ChangeUserPassword(userId, model.OldPassword, model.NewPassword);

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
    }
}
