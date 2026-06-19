namespace Skeleton.Responses;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string? ErrorCode { get; set; }
    public object? Errors { get; set; }

    public static ApiResponse<T> Success(string message, T data)
        => new()
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> Failure(string message, string errorCode)
        => new()
        {
            Message = message,
            IsSuccess = false,
            ErrorCode = errorCode,
        };
}
