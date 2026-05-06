using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using MediatR;

namespace Fixy.Application.Features.Chat.Commands.UploadAttachment;

public sealed class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, Result<string>>
{
    private readonly IStorageService _fileService;
    public UploadAttachmentCommandHandler(IStorageService fileService)
    {
        _fileService = fileService;
    }

    public async Task<Result<string>> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        return await _fileService.UploadAsync(request.Attachment);
    }
}
