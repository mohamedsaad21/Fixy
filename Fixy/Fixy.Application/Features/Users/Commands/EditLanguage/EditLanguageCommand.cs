using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Users.Commands.EditLanguage;

public sealed record EditLanguageCommand(string Language) : IRequest<Result>;