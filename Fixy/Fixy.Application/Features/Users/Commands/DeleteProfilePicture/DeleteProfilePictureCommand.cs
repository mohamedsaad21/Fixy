using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Users.Commands.DeleteProfilePicture;

public sealed record DeleteProfilePictureCommand() : IRequest<Result>;