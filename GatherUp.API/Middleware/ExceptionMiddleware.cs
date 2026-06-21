using GatherUp.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace GatherUp.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next) => _next = next;

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

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode = ex switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                AlreadyExistsException => (int)HttpStatusCode.Conflict,
                ForbiddenException => (int)HttpStatusCode.Forbidden,
                LockedEntityException => (int)HttpStatusCode.Locked,
                InvalidInputException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new { error = ex.Message });
            return context.Response.WriteAsync(result);
        }
    }
}
