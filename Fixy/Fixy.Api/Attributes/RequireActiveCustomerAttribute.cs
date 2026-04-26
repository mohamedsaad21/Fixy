using Fixy.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Attributes;

public class RequireActiveCustomerAttribute : ServiceFilterAttribute
{
    public RequireActiveCustomerAttribute() : base(typeof(CustomerStatusFilter))
    {
    }
}
