using DDStudy2022.Api.Models.Notifications;
using DDStudy2022.Api.Models.Pushes;
using DDStudy2022.Api.Services;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.Common.Extensions;
using DDStudy2022.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PushController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly GooglePushService _googlePushService;
        public PushController(UserService userService, GooglePushService pushService)
        {
            _userService = userService;
            _googlePushService = pushService;
        }

        [HttpPost]
        public async Task Subscribe(PushTokenModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _userService.SetPushToken(userId, model.Token);
        }

        [HttpPost]
        public async Task Unsubscribe()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            await _userService.SetPushToken(userId);
        }

        [Obsolete("Will be removed before merging into master")]
        [HttpPost]
        public async Task<List<string>> SendPush(string notifyType, Guid? postId, SendPushModel model)
        {
            var res = new List<string>();

            var recieverId = model.UserId 
                ?? throw new IdClaimConversionException();

            var token = await _userService.GetPushToken(recieverId);
            if (token != default)
            {
                res = _googlePushService.SendNotification(token, model.Push);
            }

            return res;
        }
    }
}
