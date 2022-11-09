using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Api.Services;
using DDStudy2022.Common;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Extensions;
using DDStudy2022.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;
        public UserController(UserService userService, AuthService authService)
        {
            _userService = userService;
            if (_userService != null)
                _userService.SetLinkGenerator(x =>
                    Url.Action(nameof(GetUserAvatar), new { userId = x.Id, download = false }));
            
            _authService = authService;
        }

        [HttpGet]
        public async Task<IEnumerable<UserAvatarModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        public async Task<UserAvatarModel> GetCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                return await _userService.GetUserModel(userId);
            }
            else
                throw new Exception("you are not authorized");
        }

        [HttpPost]
        public async Task ChangeCurrentUserPassword(PasswordChangeModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                await _userService.ChangeUserPassword(userId, model.OldPassword, model.NewPassword);
            }
            else
                throw new Exception("Seems like you don\'t exist");
        }

        [HttpPost]
        public async Task SuspendCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
                await _userService.SuspendUser(userId);
            else
                throw new Exception("Seems like user don\'t exist");
        }

        [HttpGet]
        public async Task<ICollection<SessionModel>> GetCurrentSessions()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
                return await _userService.GetUserSessionModels(userId);
            else
                throw new Exception("There are no current sessions for this user");
        }

        [HttpPost]
        public async Task DeactivateSession(SessionDeactivationRequest request) => await _authService.DeactivateSession(request.RefreshToken);

        [HttpPost]
        public async Task AddAvatarToUser(MetadataModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
                if (!tempFi.Exists)
                    throw new Exception("file not found");
                else
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", model.TempId.ToString());
                    var destFi = new FileInfo(path);
                    if (destFi.Directory != null && !destFi.Directory.Exists)
                        destFi.Directory.Create();

                    System.IO.File.Copy(tempFi.FullName, path, true);

                    await _userService.AddAvatarToUser(userId, model, path);
                }
            }
            else
                throw new Exception("Seems like you don\'t exist");
        }

        [HttpGet]
        public async Task<FileStreamResult> GetUserAvatar(Guid userId, bool download = false)
        {
            var attachment = await _userService.GetUserAvatar(userId);

            var fs = new FileStream(attachment.FilePath, FileMode.Open);
            if (download)
                return File(fs, attachment.MimeType, attachment.Name);
            else
                return File(fs, attachment.MimeType);
        }

        [HttpGet]
        public async Task<FileStreamResult> GetCurrentUserAvatar(bool download = false)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                return await GetUserAvatar(userId, download);
            }
            else
                throw new Exception("you are not authorized");
        }
    }
}
