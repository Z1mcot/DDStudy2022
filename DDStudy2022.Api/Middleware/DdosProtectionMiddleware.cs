using DDStudy2022.Api.Services;
using DDStudy2022.Common.Exceptions;

namespace DDStudy2022.Api.Middleware
{
    public class DdosProtectionMiddleware
    {
        private readonly RequestDelegate _next;

        public DdosProtectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DdosGuard guard)
        {
            var authHeader = context.Request.Headers.Authorization;
            try
            {
                guard.CheckRequest(authHeader);
                await _next(context);
            }
            catch(TooManyRequestsException)
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsJsonAsync("Too many requests, allowed up to 10 requests per second");
            }
        }
    }

    public static class DdosProtectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomDdosProtection(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorMiddleware>();
        }
    }
}
