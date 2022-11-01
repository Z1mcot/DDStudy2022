using AutoMapper;
using AutoMapper.QueryableExtensions;
using DDStudy2022.Api.Models;
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
        public async Task CreateUser(CreateUserModel model) => await _userService.CreateUser(model);

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
                throw new Exception("Something strange. Seems like you don\'t exist");
        }

        [HttpPut]
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
                throw new Exception("Something strange. Seems like you don\'t exist");
        }
    }
}
