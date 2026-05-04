using Fixy.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Attributes;

public class RequireCustomerFeedbackAttribute : ServiceFilterAttribute
{
    public RequireCustomerFeedbackAttribute() : base(typeof(CustomerFeedbackFilter))
    {
        
    }
}
