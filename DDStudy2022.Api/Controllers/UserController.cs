using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Api.Services;
using DDStudy2022.Common;
using DDStudy2022.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AttachmentService _attachmentService;
        public UserController(UserService userService, AttachmentService attachmentService)
        {
            _userService = userService;
            _attachmentService = attachmentService;
        }

        [HttpPost]
        public async Task CreateUser(CreateUserModel model)
        {
            if (await _userService.CheckUserExistence(model.Email))
                throw new Exception("User already exists");
            await _userService.CreateUser(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser()
        {
            var userId = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            if (Guid.TryParse(userId, out var id))
                return await _userService.GetUser(id);
            else
                throw new Exception("Seems like you don\'t exist");
        }

        [HttpPost]
        [Authorize]
        public async Task ChangeCurrentUserPassword(PasswordChangeModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var user = await _userService.GetUser(userId);
                await _userService.ChangeUserPassword(userId, model.OldPassword, model.NewPassword);
            }
            else
                throw new Exception("Seems like you don\'t exist");
        }

        [HttpPost]
        [Authorize]
        public async Task SuspendCurrentUser()
        {
            var userId = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            if (Guid.TryParse(userId, out var id))
                await _userService.SuspendUser(id);
            else
                throw new Exception("Seems like user don\'t exist");
        }

        [HttpGet]
        [Authorize]
        public async Task<ICollection<SessionModel>> GetCurrentSessions()
        {
            var userId = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            if (Guid.TryParse(userId, out var id))
                return await _userService.GetUserSessionModels(id);
            else
                throw new Exception("There are no current sessions for this user");
        }

        [HttpPost]
        [Authorize]
        public async Task DeactivateSession(SessionDeactivationModel model) => await _userService.DeactivateSession(model.RefreshToken);

        [HttpPost]
        [Authorize]
        public async Task AddAvatarToUser(MetadataModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var tempFile = _attachmentService.GetTempFileInfo(model);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", model.TempId.ToString());
                var destFile = new FileInfo(path);
                if (destFile.Directory != null && !destFile.Directory.Exists)
                    destFile.Directory.Create();

                System.IO.File.Copy(tempFile.FullName, path, true);

                await _userService.AddAvatarToUser(userId, model, path);

            }
            else
                throw new Exception("Seems like you don\'t exist");
        }

        [HttpGet]
        public async Task<FileResult> GetUserAvatar(Guid userId)
        {
            var attachment = await _userService.GetUserAvatar(userId);

            return File(System.IO.File.ReadAllBytes(attachment.FilePath), attachment.MimeType);
        }
    }
}
