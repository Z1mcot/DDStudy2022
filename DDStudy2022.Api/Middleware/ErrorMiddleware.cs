using DDStudy2022.Common.Exceptions;

namespace DDStudy2022.Api.Middleware
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(ex.Message);
                //await context.Response.CompleteAsync()
            }
            catch (ForbiddenActionException ex)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
            catch (PrivateAccountException ex)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode != null) 
                    context.Response.StatusCode = (int)ex.StatusCode;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
            catch (UnsupportedMimeTypeException ex)
            {
                context.Response.StatusCode = 415;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
        }
    }

    public static class ErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalErrorWrapper(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorMiddleware>();
        }
    }
}
