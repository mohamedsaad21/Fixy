namespace Fixy.Application.Bases;

public static class Errors
{
    public static Error EmailAlreadyExists =>
    new(
        Id: "EmailAlreadyExists",
        Type: ErrorType.BadRequest,
        Description: "Email already exists"
    );

    public static Error UserNameAlreadyExists =>
        new(
            Id: "UserNameAlreadyExists",
            Type: ErrorType.BadRequest,
            Description: "Username already exists"
        );

    public static Error EmailOrPasswordInCorrect =>
    new(
        Id: "EmailOrPasswordInCorrect",
        Type: ErrorType.BadRequest,
        Description: "Email or Password is incorrect!"
    );

    public static Error EmailNotConfirmed => new("EmailNotConfirmed", ErrorType.BadRequest, "Email is not confirmed!");
    public static Error UserNotFound => new("UserNotFound", ErrorType.NotFound, "User is not found!");
    public static Error InvalidCode => new("InvalidCode", ErrorType.BadRequest, "This code is invalid!");
    public static Error IdentityFailure(string description) => new("IdentityFailure", ErrorType.Failure, description);
}
