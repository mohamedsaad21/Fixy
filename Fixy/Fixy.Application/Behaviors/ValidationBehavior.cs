using Fixy.Application.Resources;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    #region Fields
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IStringLocalizer<SharedResources> _localizer;
    #endregion

    #region Constructors
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, IStringLocalizer<SharedResources> localizer)
    {
        _validators = validators;
        _localizer = localizer;
    }
    #endregion

    #region Handle Functions
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var message = failures.Select(x => _localizer[$"{x.PropertyName}"] + ":" + _localizer[x.ErrorMessage]).FirstOrDefault();

                throw new ValidationException(message);
            }
        }
        return await next();
    }
    #endregion
}