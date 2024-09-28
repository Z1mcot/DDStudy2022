using DDStudy2022.Api.Models.Tokens;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Api.Services;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/auth")]
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
        [Route("login")]
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
        [Route("refresh")]
        public async Task<TokenModel> RenewToken(RefreshTokenRequestModel model) 
            => await _authService.GetTokenByRefreshToken(model.RefreshToken);

        [HttpPost]
        [Route("register")]
        public async Task RegisterUser(CreateUserModel model)
        {
            if (await _userService.CheckUserExistence(model.Email))
                throw new Exception("user with this email already exists");
            
            await _userService.CreateUser(model);

        }
        
        [HttpPost]
        [Authorize]
        [Route("change-password")]
        public async Task ChangeCurrentUserPassword(PasswordChangeRequest request)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _userService.ChangeUserPassword(userId, request);
        }
    }
}
