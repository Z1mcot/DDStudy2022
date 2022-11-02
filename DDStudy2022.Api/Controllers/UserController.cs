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
        public UserController(UserService userService)
        {
            _userService = userService;
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
            var userId = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            if (Guid.TryParse(userId, out var id))
            {
                var user = await _userService.GetUser(id);
                await _userService.ChangeUserPassword(id, model.OldPassword, model.NewPassword);
            }
            else
                throw new Exception("Seems like you don\'t exist");
        }

        // Лучше конечно не удалять а замораживать аккаунты
        [HttpPost]
        [Authorize]
        public async Task DeleteCurrentUser()
        {
            var userId = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            if (Guid.TryParse(userId, out var id))
                await _userService.DeleteUser(id);
            else
                throw new Exception("Seems like user don\'t exist");
        }

        [HttpGet]
        [Authorize]
        public async Task<ICollection<SessionModel>> GetCurrentSessions()
        {
            var userId = User.Claims.FirstOrDefault(p => p.Type == "id")?.Value;
            if (Guid.TryParse(userId, out var id))
                return await _userService.GetUserSessions(id);
            else
                throw new Exception("There are no current sessions for this user");
        }

        [HttpPost]
        [Authorize]
        public async Task DeactivateSession(SessionDeactivationModel model) => await _userService.DeactivateSession(model.RefreshToken);
    }
}
