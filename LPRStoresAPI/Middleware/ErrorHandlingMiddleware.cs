using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // For ILogger
using System;
using System.Net;
using System.Text.Json; // For JsonSerializer
using System.Threading.Tasks;

namespace LPRStoresAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = (int)HttpStatusCode.InternalServerError; // Default to 500
            var message = "An unexpected internal server error has occurred.";

            // You can customize status codes and messages based on exception types
            if (exception is ArgumentException argumentException) // Example for specific exception type
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = argumentException.Message;
            }
            // Add more specific exception handling here if needed
            // else if (exception is UnauthorizedAccessException)
            // {
            //     statusCode = (int)HttpStatusCode.Unauthorized;
            //     message = "Unauthorized access.";
            // }
            // else if (exception is KeyNotFoundException)
            // {
            //     statusCode = (int)HttpStatusCode.NotFound;
            //     message = "Resource not found.";
            // }

            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new { error = message, statusCode = statusCode });
            return context.Response.WriteAsync(result);
        }
    }
}
