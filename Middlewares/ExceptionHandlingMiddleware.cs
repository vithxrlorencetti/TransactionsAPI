using System.Net;
using System.Text.Json;
using TransactionsAPI.Models;

namespace TransactionsAPI.Middlewares;

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
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();
        var statusCode = HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorResponse.Message = notFoundException.Message;
                break;

            case BadRequestException badRequestException:
                statusCode = HttpStatusCode.BadRequest;
                errorResponse.Message = badRequestException.Message;
                break;

            case AddressNotFoundException addressNotFoundException:
                statusCode = HttpStatusCode.BadRequest;
                errorResponse.Message = addressNotFoundException.Message;
                break;

            case UnauthorizedException unauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                errorResponse.Message = unauthorizedException.Message;
                break;

            case ConflictException conflictException:
                statusCode = HttpStatusCode.Conflict;
                errorResponse.Message = conflictException.Message;
                break;

            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation Error";
                errorResponse.Errors = validationException.Errors;
                break;

            default:
                _logger.LogError(exception, "Unhandled error: {Message}", exception.Message);
                errorResponse.Message = "An internal server error occurred.";
                errorResponse.Errors = new List<string> { $"Exception Type: {exception.GetType().FullName}" };
                errorResponse.Errors.Append(exception.StackTrace ?? "No stack trace available.");
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(result);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public IEnumerable<string>? Errors { get; set; }
}
