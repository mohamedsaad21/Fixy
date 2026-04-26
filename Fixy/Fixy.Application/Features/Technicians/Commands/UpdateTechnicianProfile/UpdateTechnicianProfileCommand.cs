using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.Technicians.Commands.UpdateTechnicianProfile;

public sealed record UpdateTechnicianProfileCommand
    (
        Guid TechnicianId,
        string NationalId,
        IFormFile NationalIdCardImage
    ) : IRequest<Result>;