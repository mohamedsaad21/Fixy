namespace Fixy.Api.Contracts.Routing;

public static class Router
{
    public const string root = "Api";
    public const string version = "v1";
    public const string Rule = root + "/" + version;

    public static class AuthenticationRouting
    {
        public const string prefix = Rule + "/Authentication";
        public const string RegisterCustomer = prefix + "/Register-Customer";
        public const string RegisterTechnician = prefix + "/Register-Technician";
        public const string SignIn = prefix + "/SignIn";
        public const string CustomersList = prefix + "/Customers-List";
        public const string SendConfirmEmail = prefix + "/Send-Confirm-Email";
        public const string ConfirmEmail = prefix + "/Confirm-Email";
        public const string RefreshToken = prefix + "/Refresh-Token";
        public const string RevokeToken = prefix + "/Revoke-Token";
        public const string SendResetPassword = prefix + "/Send-Reset-Password";
        public const string ConfirmResetPassword = prefix + "/Confirm-Reset-Password";
        public const string ResetPassword = prefix + "/Reset-Password";
    }
}
