namespace Fixy.Domain.AppMetaData;

public static class Router
{
    public const string root = "Api";
    public const string version = "v1";
    public const string Rule = root + "/" + version;

    public static class AuthenticationRouting
    {
        public const string prefix = Rule + "/Authentication";
        public const string RegisterCustomer = prefix + "/Register-Customer";
        public const string SignIn = prefix + "/SignIn";
        public const string CustomersList = prefix + "/Customers-List";
        public const string ConfirmEmail = prefix + "/Confirm-Email";
    }
}
