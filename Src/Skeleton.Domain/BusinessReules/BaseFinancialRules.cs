using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.BusinessRules;

public class BaseFinancialRules<TEntity> where TEntity : BaseEntity
{
    public void EnsureAmountPositive(decimal amount)
    {
        if (amount <= 0)
            throw new BusinessException(ErrorType.Validation, "Amount must be positive");
    }
}