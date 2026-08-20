using System.Net;

namespace LodgingReservation_BE
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext ctx)
        {
            try
            {
                await _next(ctx); // teruskan ke pipeline
            }
            catch (Exception ex)
            {
                // tangkap semua error yang lolos
                ctx.Response.ContentType = "application/json";
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    status = "error",
                    message = "Terjadi Kesalahan Server"
                });
            }
        }
    }
}
