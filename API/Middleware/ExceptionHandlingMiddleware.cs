using System.Net;
using System.Text.Json;

namespace API.Middleware;

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
            _logger.LogError(ex, "Unhandled exception processing request {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var resultObj = new { error = new { message = "An unexpected error occurred.", detail = exception.Message } };

            if (exception is TimeoutException)
            {
                code = HttpStatusCode.GatewayTimeout;
            }
            else if (exception is HttpRequestException httpEx)
            {
                code = HttpStatusCode.BadGateway;
                resultObj = new { error = new { message = httpEx.Message, detail = (string?)null } };
            }
            else if (exception is InvalidOperationException)
            {
                code = HttpStatusCode.BadRequest;
                resultObj = new { error = new { message = exception.Message, detail = (string?)null } };
            }

            var result = JsonSerializer.Serialize(resultObj);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
}
