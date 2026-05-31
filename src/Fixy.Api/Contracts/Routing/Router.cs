namespace Fixy.Api.Contracts.Routing;

public static class Router
{
    public const string root = "Api";
    public const string version = "v{version:apiVersion}";
    public const string Rule = root + "/" + version;

    public const string SingleRoute = "/{Id}";

    public static class AuthenticationRouting
    {
        public const string prefix = Rule + "/Authentication";
        public const string RegisterCustomer = prefix + "/Register-Customer";
        public const string RegisterTechnician = prefix + "/Register-Technician";
        public const string SignIn = prefix + "/SignIn";
        public const string Enable2FA = prefix + "/Enable-2FA";
        public const string Disable2FA = prefix + "/Disable-2FA";
        public const string VerifyOtp = prefix + "/Verify-Otp";
        public const string CustomersList = prefix + "/Customers-List";
        public const string SendConfirmEmail = prefix + "/Send-Confirm-Email";
        public const string ConfirmEmail = prefix + "/Confirm-Email";
        public const string RefreshToken = prefix + "/Refresh-Token";
        public const string RevokeToken = prefix + "/Revoke-Token";
        public const string SendResetPassword = prefix + "/Send-Reset-Password";
        public const string ConfirmResetPassword = prefix + "/Confirm-Reset-Password";
        public const string ResetPassword = prefix + "/Reset-Password";
        public const string ChangePassword = prefix + "/Change-Password";
        public const string SignInWithGoogleAsync = prefix + "/Google-SignIn";
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
        public const string ServiceRequestPaginatedList = prefix + "/Service-Requests-Paginated-List";
        public const string CustomerServiceRequestsPaginated = prefix + "/Customer-Service-Requests-Paginated";
        public const string CustomerServiceRequestById = prefix + "/Customer-Service-Request" + SingleRoute;
        public const string GetById = prefix + "/GetById" + SingleRoute;
        public const string Create = prefix + "/Create";
        public const string Edit = prefix + "/Edit";
        public const string Delete = prefix + "/Delete" + SingleRoute;
        public const string AddServiceRequestImages = prefix + "/Add-Images";
        public const string DeleteServiceRequestImageById = prefix + "/Delete-Image-ById/{ImageId}";
    }

    public static class TechnicianRouting
    {
        public const string prefix = Rule + "/Technician";
        public const string GetById = prefix + "/GetById" + SingleRoute;
        public const string GetTechnicianProfileForCustomers = prefix + "/Get-Technician-Profile-For-Customers/{TechnicianId}";
        public const string GetCustomerProfileForTechnicians = prefix + "/Get-Customer-Profile-For-Technicians/{CustomerId}";
        public const string TechnicianServiceRequestsList = prefix + "/Technician-Service-Requests-Paginated";
        public const string TechnicianSubmittedServiceRequestsList = prefix + "/Technician-Submitted-Service-Requests-Paginated";
        public const string ServiceRequestById = prefix + "/Service-Request-By-Id/{Id}";
        public const string Location = prefix + "/Location";
        public const string UpdateTechnicianProfile = prefix + "/update-technician-profile";
        public const string TechnicianStripeStatus = prefix + "/technician-stripe-status";
    }
    public static class PriceOfferRouting
    {
        public const string prefix = Rule + "/Price-Offer";
        public const string CreatePriceOffer = prefix + "/Create";
        public const string AcceptPriceOffer = prefix + "/Accept-Price-Offer/{PriceOfferId}";
    }
    public static class BookingRouting
    {
        public const string prefix = Rule + "/Booking";
        public const string CustomerPaginatedList = prefix + "/Customer-PaginatedList";
        public const string TechnicianPaginatedList = prefix + "/Technician-PaginatedList";
        public const string GetByIdForCustomer = prefix + "/Get-By-Id-For-Customer" + SingleRoute;
        public const string GetByIdForTechnician = prefix + "/Get-By-Id-For-Technician" + SingleRoute;
        public const string RequestPriceChange = prefix + "/request-price-change";
        public const string ApprovePriceChange = prefix + "/approve-price-change/{BookingId}";
        public const string RejectPriceChange = prefix + "/reject-price-change/{BookingId}";
        public const string MarkBookingCompleted = prefix + "/mark-booking-completed";
        public const string ConfirmBookingCompletion = prefix + "/confirm-booking-completion/{BookingId}";
        public const string CancelBookingByCustomer = prefix + "/cancel-booking-by-customer";
        public const string CancelBookingByTechnician = prefix + "/cancel-booking-by-technician";
    }
    public static class PaymentRouting
    {
        public const string prefix = Rule + "/Payments";
        public const string Create = prefix + "/Create";
        public const string Callback = prefix + "/Callback";
        public const string confirmCashReceipt = prefix + "/confirm-cash-receipt/{BookingId}";
        public const string PayCommissions = prefix + "/Pay-Commissions";
        public const string GetPendingCommissions = prefix + "/Get-Pending-Commissions";
        public const string GetTechnicianCashCommissionsOwed = prefix + "/Get-Technician-Cash-Commissions-Owed";
    }

    public static class AdminRouting
    {
        public const string prefix = Rule + "/Admin";
        public const string ApproveTechnician = prefix + "/Approve-Technician/{TechnicianId}";
        public const string RejectTechnician = prefix + "/Reject-Technician";
        public const string BlockTechnician = prefix + "/Block-Technician";
        public const string BlockCustomer = prefix + "/Block-Customer";
        public const string UnblockCustomer = prefix + "/Unblock-Customer/{CustomerId}";
        public const string UnblockTechnician = prefix + "/Unblock-Technician/{TechnicianId}";
        public const string GetTechnicians = prefix + "/Get-Technicians";
        public const string GetCustomers = prefix + "/Get-Customers";
        public const string GetBookings = prefix + "/Get-Bookings";
        public const string GetBookingById = prefix + "/Get-Booking-By-Id/{Id}";
        public const string GetUserInfoById = prefix + "/Get-User-Info-By-Id/{UserId}";
    }
    public static class NotificationsRouting
    {
        public const string prefix = Rule + "/Notifications";
        public const string PaginatedList = prefix + "/Paginated-List";
        public const string SendNotification = prefix + "/Send-Notification";
        public const string MarkAsRead = prefix + "/Mark-As-Read/{NotificationId}";
        public const string SaveFcmToken = prefix + "/Save-Fcm-Token";
        public const string Test = prefix + "/Test";
    }

    public static class FeedbackRouting
    {
        public const string prefix = Rule + "/Feedback";
        public const string SubmitCustomerFeedback = prefix + "/Submit-Customer-Feedback";
        public const string SubmitTechnicianFeedback = prefix + "/Submit-Technician-Feedback";
        public const string GetBookingFeedbacks = prefix + "/Get-Booking-Feedbacks/{BookingId}";
        public const string GetPendingCustomerFeedbackStatus = prefix + "/Get-Pending-Customer-Feedback-Status";
        public const string GetPendingTechnicianFeedbackStatus = prefix + "/Get-Pending-Technician-Feedback-Status";
    }

    public static class UsersRouting
    {
        public const string prefix = Rule + "/Users";
        public const string DeleteProfilePicture = prefix + "/Delete-Profile-Picture";
        public const string GetUserProfileById = prefix + "/Get-User-Profile-By-Id/{Id}";
        public const string EditUserProfile = prefix + "/Edit-User-Profile";
        public const string Me = prefix + "/Me";
    }

    public static class ChatRouting
    {
        public const string prefix = Rule + "/Chat";
        public const string GetMessages = prefix + "/Get-Messages";
        public const string MarkMessagesAsRead = prefix + "/Mark-Messages-As-Read/{BookingId}";
        public const string UploadAttachment = prefix + "/Upload-Attachment";
    }
    public static class DashboardRouting
    {
        public const string prefix = Rule + "/Dashboard";
        public const string GetAdminDashboard = prefix + "/Get-Admin-Dashboard";
        public const string GetTechnicianDashboard = prefix + "/Get-Technician-Dashboard";
        public const string GetCustomerDashboard = prefix + "/Get-Customer-Dashboard";
    }

    public static class ChatbotRouting
    {
        public const string prefix = Rule + "/Chatbot";
        public const string SendPrompt = prefix + "/Send-Prompt";
    }
}