using Skeleton.Domain.Eunm;

namespace Skeleton.Domain.Exceptions;

public class BusinessException : Exception
{
    public ErrorType ErroType { get; }
    public string? EntityName { get; }
    public Guid? EntityId { get; }

    public BusinessException(ErrorType type, string? message = null, string? entityName = null, Guid? entityId = null)
        : base(message)
    {
        ErroType = type;
        EntityName = entityName;
        EntityId = entityId;
    }

}