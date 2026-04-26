using Fixy.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Attributes;

public class RequireActiveTechnicianAttribute : ServiceFilterAttribute
{
    public RequireActiveTechnicianAttribute() : base(typeof(TechnicianStatusFilter))
    {
        
    }
}
