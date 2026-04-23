using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Commands.CancelBookingByCustomer;

public class CancelBookingByCustomerCommandValidator : AbstractValidator<CancelBookingByCustomerCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public CancelBookingByCustomerCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.Reason).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .MustAsync(async (model, key, CancellationToken) => Enum.IsDefined(typeof(CustomerCancellationReason), key))
            .WithMessage(_stringLocalizer[SharedResourcesKeys.InvalidCancellationReason]);
    }
}
