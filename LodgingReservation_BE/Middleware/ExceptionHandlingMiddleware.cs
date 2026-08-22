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
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";

            if (exception is NotFoundException || exception is ResourceNotFoundException)
            {
                code = HttpStatusCode.NotFound;
                message = exception.Message;
            }
            else if (exception is ConflictException)
            {
                code = HttpStatusCode.Conflict;
                message = exception.Message;
            }
            else if (exception is ValidationException || exception is RoomNotAvailableException)
            {
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exception is UnauthorizedAppException || exception is UnauthorizedAccessException)
            {
                code = HttpStatusCode.Unauthorized;
                message = exception.Message;
            }
            else if (exception is ArgumentException || exception is InvalidOperationException)
            {
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var result = JsonSerializer.Serialize(new
            {
                statusCode = (int)code,
                message = message,
                timestamp = DateTime.UtcNow
            });

            return context.Response.WriteAsync(result);
        }
    }
}