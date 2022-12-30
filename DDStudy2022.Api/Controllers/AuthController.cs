using DDStudy2022.Api.Models.Tokens;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Api.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public AuthController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<TokenModel> GenerateToken(TokenRequestModel model)
        {
            try
            {
                var token = await _authService.GetToken(model.Login, model.Password, model.Ip);
                return token;
            }
            catch (Exception)
            {
                throw new HttpRequestException("not authorized", null, statusCode: System.Net.HttpStatusCode.Unauthorized);
            }
        }

        [HttpPost]
        public async Task<TokenModel> RenewToken(RefreshTokenRequestModel model) 
            => await _authService.GetTokenByRefreshToken(model.RefreshToken);

        [HttpPost]
        public async Task RegisterUser(CreateUserModel model)
        {
            if (await _userService.CheckUserExistence(model.Email))
                throw new Exception("user with this email already exists");
            await _userService.CreateUser(model);

        }
    }
}
