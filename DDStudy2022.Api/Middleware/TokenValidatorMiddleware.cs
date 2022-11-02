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
