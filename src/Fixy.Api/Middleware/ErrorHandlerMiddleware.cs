using Azure;
using Fixy.Application.Bases;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Fixy.Api.Middleware;

public sealed class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.Message, context.Request, "");
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        Error error = exception switch
        {
            UnauthorizedAccessException => new Error("Unauthorized", ErrorType.Unauthorized, "Unauthorized access"),

            ValidationException => new Error("ValidationError", ErrorType.Validation, exception.Message),

            FluentValidation.ValidationException fvEx => new Error("ValidationError", ErrorType.Validation, exception.Message),

            KeyNotFoundException => new Error("NotFound", ErrorType.NotFound, exception.Message),

            DbUpdateException => new Error("DatabaseError", ErrorType.Failure, "Database operation failed"),

            RequestFailedException azEx when azEx.Status == 404 => new Error("NotFound", ErrorType.NotFound, "File not found in storage"),

            RequestFailedException => new Error("StorageError", ErrorType.Failure, "Storage operation failed"),

            ArgumentNullException argNullEx => new Error("InvalidArgument", ErrorType.BadRequest, argNullEx.Message),

            ArgumentException argEx => new Error("InvalidArgument", ErrorType.BadRequest, argEx.Message),

            _ => new Error("ServerError", ErrorType.Failure, "An unexpected error occurred")
        };

        var statusCode = MapToStatusCode(error.Type);

        context.Response.StatusCode = (int)statusCode;

        var result = Result.Failure(error);

        var json = JsonSerializer.Serialize(result);

        await context.Response.WriteAsync(json);
    }

    private static HttpStatusCode MapToStatusCode(ErrorType type) =>
        type switch
        {
            ErrorType.BadRequest => HttpStatusCode.BadRequest,
            ErrorType.Validation => HttpStatusCode.BadRequest,
            ErrorType.NotFound => HttpStatusCode.NotFound,
            ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
            ErrorType.Failure => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError
        };
}