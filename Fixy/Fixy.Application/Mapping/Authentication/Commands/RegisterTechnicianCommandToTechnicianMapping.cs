using Fixy.Application.Features.Authentication.Commands.Models;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Authentication;

public partial class AuthenticationProfile
{
    public void RegisterTechnicianCommandToTechnicianMapping()
    {
        CreateMap<RegisterTechnicianCommand, Technician>();
    }
}
