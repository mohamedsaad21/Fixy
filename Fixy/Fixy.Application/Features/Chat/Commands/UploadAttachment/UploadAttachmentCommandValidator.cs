using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Chat.Commands.UploadAttachment;

public class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    public UploadAttachmentCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Attachment)
        .NotNull()
        .WithMessage(_stringLocalizer[SharedResourcesKeys.Required])

        .Must(file => file.Length <= 10 * 1024 * 1024)
        .WithMessage("File size must not exceed 10MB")

        .Must(file =>
        {
            var allowedExtensions = new[]
            {
                ".jpg", ".jpeg", ".png", // images
                ".pdf", ".doc", ".docx"  // documents
            };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        })
        .WithMessage("Allowed file types: .jpg, .jpeg, .png, .pdf, .doc, .docx")

        .Must(file =>
        {
            var allowedContentTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

            return allowedContentTypes.Contains(file.ContentType.ToLowerInvariant());
        })
        .WithMessage("Invalid file type");
    }
}