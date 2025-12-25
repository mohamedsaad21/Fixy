using Fixy.Application.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Fixy.Application.Middleware;

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

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        Error error = exception switch
        {
            UnauthorizedAccessException =>
                new Error(
                    "Unauthorized",
                    ErrorType.Unauthorized,
                    "Unauthorized access"),

            ValidationException =>
                new Error(
                    "ValidationError",
                    ErrorType.Validation,
                    exception.Message),

            FluentValidation.ValidationException fvEx =>
                new Error(
                    "ValidationError",
                    ErrorType.Validation,
                    exception.Message
                ),

            KeyNotFoundException =>
                new Error(
                    "NotFound",
                    ErrorType.NotFound,
                    exception.Message),

            DbUpdateException =>
                new Error(
                    "DatabaseError",
                    ErrorType.Failure,
                    "Database operation failed"),

            _ =>
                new Error(
                    "ServerError",
                    ErrorType.Failure,
                    "An unexpected error occurred")
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
