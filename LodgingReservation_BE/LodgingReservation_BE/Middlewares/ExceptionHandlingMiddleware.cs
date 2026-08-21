using System.Net;
using System.Text.Json;
using LodgingReservation_BE.Exceptions;

namespace LodgingReservation_BE.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                ValidationException ex => (HttpStatusCode.BadRequest, ex.Message),
                NotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
                ConflictException ex => (HttpStatusCode.Conflict, ex.Message),
                UnauthorizedAppException ex => (HttpStatusCode.Unauthorized, ex.Message),

                ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),
                KeyNotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
                InvalidOperationException ex => (HttpStatusCode.Conflict, ex.Message),

                _ => (HttpStatusCode.InternalServerError, "Terjadi kesalahan pada server.")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception pada {Method} {Path}",
                    context.Request.Method, context.Request.Path);
            }
            else
            {
                _logger.LogWarning("{ExceptionType} pada {Method} {Path}: {Message}",
                    exception.GetType().Name, context.Request.Method, context.Request.Path, exception.Message);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var payload = JsonSerializer.Serialize(new
            {
                status = "error",
                message
            });

            await context.Response.WriteAsync(payload);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseAppExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
