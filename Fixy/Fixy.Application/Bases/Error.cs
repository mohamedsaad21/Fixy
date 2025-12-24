namespace Fixy.Application.Bases;

public record Error(string Id, ErrorType Type, string Description);