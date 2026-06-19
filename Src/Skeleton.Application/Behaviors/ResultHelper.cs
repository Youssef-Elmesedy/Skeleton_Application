using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Behaviors;

public static class ResultHelper
{
    public static Result<T> FromBusinessException<T>(BusinessException ex)
    {
        return ex.ErroType switch
        {
            ErrorType.Conflict =>
                Result<T>.Failure("Something went wrong.", CommonErrors.Conflict(ex.Message ?? "Conflict occurred.")),

            ErrorType.Validation =>
                Result<T>.Failure("Something went wrong.", CommonErrors.Validation(ex.Message ?? "Validation failed.")),

            ErrorType.NotFound =>
            Result<T>.Failure("Something went wrong.",
                ex.EntityName != null
                    ? CommonErrors.NotFound(ex.EntityName, ex.EntityId)
                    : CommonErrors.NotFound(ex.Message ?? "Not Found")
            ),

            _ =>
                Result<T>.Failure("Something went wrong.", CommonErrors.Failure($"{ex.Message}"))
        };
    }
}
