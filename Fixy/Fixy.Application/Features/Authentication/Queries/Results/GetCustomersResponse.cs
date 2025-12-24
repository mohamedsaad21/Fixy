namespace Fixy.Application.Features.Authentication.Queries.Results;

public class GetCustomersResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}
