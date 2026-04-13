namespace Fixy.Application.Features.Admin.Commands.ApproveTechnician;

public class ApproveTechnicianResponse
{
    public bool IsSuccess { get; set; }
    public Guid TechnicianId {  get; set; }
    public string StripeAccountId {  get; set; }
    public string OnboardingUrl {  get; set; }
    public string Message {  get; set; }
}
