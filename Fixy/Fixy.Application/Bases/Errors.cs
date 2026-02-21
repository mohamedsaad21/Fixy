namespace Fixy.Application.Bases;

public static class Errors
{
    public static Error EmailAlreadyExists => new("EmailAlreadyExists", ErrorType.BadRequest, "Email already exists");
    public static Error IdentityCreateUserFailed => new("IdentityCreateUserFailed", ErrorType.BadRequest, "Failed to create user");
    public static Error Unauthorized => new("Unauthorized", ErrorType.Unauthorized, "You are not authorized to perform this action");
    public static Error IdentityAddRoleFailed => new("IdentityAddRoleFailed", ErrorType.BadRequest, "Failed to add role");
    public static Error PhoneNumberAlreadyExists => new("PhoneNumberAlreadyExists", ErrorType.BadRequest, "Phone number already exists");
    public static Error NationalIdAlreadyExists => new("NationalIdAlreadyExists", ErrorType.BadRequest, "National Id already exists");
    public static Error FileUploadFailed => new("FileUploadFailed", ErrorType.BadRequest, "Failed to upload file!");
    public static Error UserNameAlreadyExists => new("UserNameAlreadyExists", ErrorType.BadRequest, "Username already exists");
    public static Error EmailOrPasswordInCorrect => new(Id: "EmailOrPasswordInCorrect", Type: ErrorType.BadRequest, Description: "Email or Password is incorrect!");
    public static Error EmailNotConfirmed => new("EmailNotConfirmed", ErrorType.BadRequest, "Email is not confirmed!");
    public static Error EmailAlreadyConfirmed => new("EmailAlreadyConfirmed", ErrorType.BadRequest, "Email is already confirmed!");
    public static Error UserNotFound => new("UserNotFound", ErrorType.NotFound, "User is not found!");
    public static Error InvalidCode => new("InvalidCode", ErrorType.BadRequest, "This code is invalid!");
    public static Error InvalidToken => new("InvalidToken", ErrorType.BadRequest, "This token is invalid!");
    public static Error InactiveToken => new("InactiveToken", ErrorType.BadRequest, "This token is inactive!");
    public static Error RequestInsertionFailed => new("InsertionFailed", ErrorType.BadRequest, "Failed to insert Request");
    public static Error LocationNotUpdated => new("LocationNotUpdated", ErrorType.BadRequest, "You must update your location");
    public static Error ServiceRequestNotFound => new("ServiceRequestNotFound", ErrorType.NotFound, "Service request is not found");
    public static Error AlreadyCreatedPriceOffer => new("AlreadyCreatedPriceOffer", ErrorType.BadRequest, "You have already created price offer for this service request");
    public static Error PriceOfferNotFound => new("PriceOfferNotFound", ErrorType.BadRequest, "Price offer is not found");
    public static Error ServiceAlreadyAssigned => new("ServiceAlreadyAssigned", ErrorType.BadRequest, "This service already assigned to a technician");
    public static Error BookingNotFound => new("BookingNotFound", ErrorType.NotFound, "Booking is not found");
    public static Error BookingNotActive => new("BookingNotActive", ErrorType.BadRequest, "Booking is not active");
    public static Error PriceChangeAlreadyPending => new("PriceChangeAlreadyPending", ErrorType.BadRequest, "Price change is already pending");
    public static Error InvalidBookingState => new("InvalidBookingState", ErrorType.BadRequest, "Booking state is invalid");
    public static Error NoPriceChangeToApprove => new("NoPriceChangeToApprove", ErrorType.BadRequest, "There is no price change to approve");
    public static Error AlreadyAgreedPrice => new("AlreadyAgreedPrice", ErrorType.BadRequest, "This price is already the agreed price");
    public static Error ImageNotFound => new("ImageNotFound", ErrorType.BadRequest, "This image is not found");
    public static Error TechnicianNotFound => new("TechnicianNotFound", ErrorType.NotFound, "This technician is not found");
    public static Error TechnicianAlreadyApproved => new("TechnicianAlreadyApproved", ErrorType.BadRequest, "This technician is already approved");
    public static Error PaymentCreationFailed => new("PaymentCreationFailed", ErrorType.BadRequest, "Failed to create payment");
    public static Error BookingNotReadyForPayment => new("BookingNotReadyForPayment", ErrorType.BadRequest, "This booking is not ready for payment");
    public static Error PaymentAlreadyCompleted => new("PaymentAlreadyCompleted", ErrorType.BadRequest, "Payment is already completed");
    public static Error InvalidHmacSignature => new("InvalidHmacSignature", ErrorType.BadRequest, "Invalid HMAC signature in Paymob callback");
    public static Error InvalidMerchantOrderId => new("InvalidMerchantOrderId", ErrorType.BadRequest, "Merchant OrderId is invalid");
    public static Error PaymentNotFound => new("InvalidMerchantOrderId", ErrorType.NotFound, "Payment is not found");
    public static Error CallbackProcessingFailed => new("CallbackProcessingFailed", ErrorType.BadRequest, "Error processing Paymob callback");
    public static Error BookingNotCompleted => new("CallbackProcessingFailed", ErrorType.BadRequest, "Feedback can only be submitted after the booking is marked as completed");
    public static Error FeebackAlreadySubmitted => new("CallbackProcessingFailed", ErrorType.BadRequest, "Feedback has already been submitted for this booking");
    public static Error NotificationSendFailed => new("NotificationSendFailed", ErrorType.BadRequest, "The notification could not be delivered");
    public static Error NotificationNotFound => new("NotificationNotFound", ErrorType.NotFound, "This notification is not found");
}
