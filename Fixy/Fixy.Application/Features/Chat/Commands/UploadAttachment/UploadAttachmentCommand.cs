using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.Chat.Commands.UploadAttachment;

public sealed record UploadAttachmentCommand(IFormFile Attachment) : IRequest<Result<string>>;
