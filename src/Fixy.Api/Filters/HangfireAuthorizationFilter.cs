using Fixy.Domain.Constants;
using Hangfire.Dashboard;

namespace Fixy.Api.Filters;

public class HangfireAuthorizationFilter :
IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        // Only allow authenticated admins
        return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.IsInRole(Roles.Admin);
    }
}