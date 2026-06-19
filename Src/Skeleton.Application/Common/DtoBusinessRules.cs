using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Common;

public static class DtoBusinessRules
{
    public static void EnsureExists<T>(T? dto, Guid id, string entityName)
    {
        if (dto is null)
            throw new BusinessException(
                ErrorType.NotFound,
                entityName: entityName,
                entityId: id
            );
    }

    public static void EnsureExists<T>(T? dto, string message)
    {
        if (dto is null)
            throw new BusinessException(
                ErrorType.NotFound,
                message: message
            );
    }
}