using Fixy.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Attributes;

public class RequireTechnicianFeedbackAttribute : ServiceFilterAttribute
{
    public RequireTechnicianFeedbackAttribute() : base(typeof(TechnicianFeedbackFilter))
    {
        
    }
}
