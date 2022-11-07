using DDStudy2022.Api.Services;
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

        public async Task InvokeAsync(HttpContext context, UserService userService)
        {
            var isOk = true;
            var sessionIdString = context.User.Claims.FirstOrDefault(x => x.Type == "sessionId")?.Value;
            if (Guid.TryParse(sessionIdString, out var sessionId))
            {
                var session = await userService.GetSessionById(sessionId);
                var user = session.User;
                // Проверка на бан, нужна просто для того чтобы выводилось правильное сообщение.
                // При suspend'е аккаунта мы отключаем все сессии
                if (!user.IsActive)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("user account is suspended");
                }

                if (!session.IsActive)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("session is not active");
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
