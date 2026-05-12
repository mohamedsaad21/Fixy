using Fixy.Application.Bases;
using Fixy.Application.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Fixy.Api.Controllers.Common;

[ApiController]
[EnableRateLimiting("sliding")]
public abstract class AppControllerBase : ControllerBase
{
    private IMediator? _mediator;
    private IStringLocalizer<SharedResources>? _localizer;

    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected IStringLocalizer<SharedResources> Localizer =>
        _localizer ??= HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResources>>();

    protected IActionResult ToActionResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        var statusCode = MapToStatusCode(result.Error!.Type);
        var localizedResult = GetLocalizedResult(result.Error);

        return StatusCode((int)statusCode, localizedResult);
    }

    protected IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        var statusCode = MapToStatusCode(result.Error!.Type);
        var localizedResult = GetLocalizedResult(result.Error);

        return StatusCode((int)statusCode, localizedResult);
    }

    private object GetLocalizedResult(Error error)
    {
        var localizedString = Localizer[error.Id];

        var localizedDescription = localizedString.ResourceNotFound
            ? error.Description ?? error.Id
            : localizedString.Value;

        return new
        {
            IsSuccess = false,
            Error = new
            {
                error.Id,
                error.Type,
                Description = localizedDescription
            }
        };
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