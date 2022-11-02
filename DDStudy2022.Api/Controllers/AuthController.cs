using DDStudy2022.Api.Models.Tokens;
using DDStudy2022.Api.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            this._userService = userService;
        }

        [HttpPost]
        public async Task<TokenModel> GenerateToken(TokenRequestModel model) 
            => await _userService.GetToken(model.Login, model.Password);

        [HttpPost]
        public async Task<TokenModel> RenewToken(RefreshTokenRequestModel model) 
            => await _userService.GetTokenByRefreshToken(model.RefreshToken);
    }
}
