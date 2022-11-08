using DDStudy2022.Api.Services;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;

namespace DDStudy2022.Api.Middleware
{
    public class TokenValidatorMiddleware
    {
        private readonly RequestDelegate _next;
        public TokenValidatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AuthService authService)
        {
            var isOk = true;
            var sessionId = context.User.GetClaimValue<Guid>(ClaimNames.SessionId);
            if (sessionId != default)
            {
                var session = await authService.GetSessionById(sessionId);
                if (!session.IsActive)
                {
                    isOk = false;
                    context.Response.Clear();
                    context.Response.StatusCode = 401;
                }
            }
            if (isOk)
            {
                await _next(context);
            }
        }
    }

    public static class TokenValidatorMiddlewareExstension
    {
        public static IApplicationBuilder UseTokenValidator(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidatorMiddleware>();
        }
    }
}
