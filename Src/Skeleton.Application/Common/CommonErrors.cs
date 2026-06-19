namespace Skeleton.Application.Common;

public static class CommonErrors
{
    // NotFound by entity name + optional id
    public static Error NotFound(string entityName, Guid? id = null)
    {
        var idPart = id.HasValue ? $" with id '{id}'" : string.Empty;
        return new Error("Not Found", $"{entityName}{idPart} was not found.");
    }

    // NotFound by custom message
    public static Error NotFound(string message)
        => new Error("Not Found", message);

    public static Error Validation(string message)
        => new Error("Validation Error", message);

    public static Error Conflict(string message)
        => new Error("Conflict Error", message);

    public static Error Failure(string message)
        => new Error("Failure", message);
}