namespace Fixy.Application.Bases;

public static class Errors
{
    public static Error EmailAlreadyExists => new("EmailAlreadyExists", ErrorType.BadRequest, "Email already exists");
    public static Error IdentityCreateUserFailed => new("IdentityCreateUserFailed", ErrorType.BadRequest, "Failed to create user");
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
}
