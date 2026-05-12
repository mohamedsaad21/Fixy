using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt;

public class ConfirmCashReceiptCommandValidator : AbstractValidator<ConfirmCashReceiptCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public ConfirmCashReceiptCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
