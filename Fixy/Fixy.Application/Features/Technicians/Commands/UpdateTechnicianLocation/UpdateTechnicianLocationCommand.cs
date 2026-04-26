using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Technicians.Commands.UpdateTechnicianLocation;

public sealed record UpdateTechnicianLocationCommand(double Latitude, double Longitude, string ServiceArea) : IRequest<Result>;
