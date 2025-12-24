using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fixy.Api.Base;

[ApiController]
public abstract class AppControllerBase : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected IActionResult ToActionResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        var statusCode = MapToStatusCode(result.Error!.Type);

        return StatusCode((int)statusCode, result);
    }

    protected IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        var statusCode = MapToStatusCode(result.Error!.Type);

        return StatusCode((int)statusCode, result);
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
