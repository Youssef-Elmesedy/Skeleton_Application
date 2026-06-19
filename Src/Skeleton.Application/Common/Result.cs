namespace Skeleton.Application.Common;

public record Error(string Code, string Message);

public class Result<T>
{
    public bool IsSuccess { get; }
    public string? Message { get; set; }
    public T? Value { get; }
    public Error? Error { get; }

    private Result(string message, bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Message = message;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(string message, T value)
        => new(message, true, value, null);

    public static Result<T> Failure(string message, Error error)
        => new(message, false, default, error);
}
