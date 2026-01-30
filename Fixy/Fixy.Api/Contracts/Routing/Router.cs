namespace Fixy.Api.Contracts.Routing;

public static class Router
{
    public const string root = "Api";
    public const string version = "v1";
    public const string Rule = root + "/" + version;

    public const string SingleRoute = "/{Id}";

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

    public static class CategoryRouting
    {
        public const string prefix = Rule + "/Category";
        public const string List = prefix + "/List";
        public const string GetById = prefix + "/GetById" + SingleRoute;
        public const string Create = prefix + "/Create";
        public const string Edit = prefix + "/Edit";
        public const string Delete = prefix + "/Delete" + SingleRoute;
    }
    public static class ServiceRequestRouting
    {
        public const string prefix = Rule + "/Request";
        public const string ServiceRequestsList = prefix + "/Service-Requests-List";
        public const string CustomerServiceRequestsList = prefix + "/Customer-Service-Requests-List";
        public const string CustomerServiceRequestById = prefix + "/Customer-Service-Request" + SingleRoute;
        public const string CreatePriceOffer = prefix + "/price-offers";
        public const string GetById = prefix + "/GetById" + SingleRoute;
        public const string Create = prefix + "/Create";
        public const string Edit = prefix + "/Edit";
        public const string Delete = prefix + "/Delete" + SingleRoute;
    }

    public static class TechnicianRouting
    {
        public const string prefix = Rule + "/Technician";
        public const string TechnicianServiceRequestsList = prefix + "/Technician-Service-Requests-List";
        public const string Location = prefix + "/Location";
    }
    public static class PriceOfferRouting
    {
        public const string prefix = Rule + "/Price-Offer";
        public const string AcceptPriceOffer = prefix + "/Accept-Price-Offer/{PriceOfferId}";
    }
    public static class BookingRouting
    {
        public const string prefix = Rule + "/Booking";
        public const string List = prefix + "/List";
        public const string GetById = prefix + "/GetById" + SingleRoute;
        public const string RequestPriceChange = prefix + "/request-price-change";
        public const string ApprovePriceChange = prefix + "/approve-price-change/{BookingId}";
        public const string RejectPriceChange = prefix + "/reject-price-change/{BookingId}";
        public const string MarkBookingCompleted = prefix + "/mark-booking-completed/{BookingId}";
        public const string ConfirmBookingCompletion = prefix + "/confirm-booking-completion/{BookingId}";
    }
}
