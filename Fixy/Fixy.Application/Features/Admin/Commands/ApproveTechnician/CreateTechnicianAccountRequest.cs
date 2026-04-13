namespace Fixy.Application.Features.Admin.Commands.ApproveTechnician;

public class CreateTechnicianAccountRequest
{
    public Guid TechnicianId { get; set; }
    public string Country { get; set; }
}
