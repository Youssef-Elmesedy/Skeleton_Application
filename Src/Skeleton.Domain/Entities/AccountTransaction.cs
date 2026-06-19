using Skeleton.Domain.Eunm;

namespace Skeleton.Domain.Entities;

public class AccountTransaction : BaseEntity
{
    public Guid CustomerId { get; private set; }

    // ✅ أضفنا AccountId عشان الـ FK يبقى صح
    public Guid AccountId { get; private set; }

    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public string Description { get; private set; } = null!;
    public DateTime Date { get; private set; } = DateTime.UtcNow;

    public AccountTransaction(
        Guid customerId,
        Guid accountId,
        decimal amount,
        TransactionType type,
        string description)
    {
        CustomerId = customerId;
        AccountId = accountId;
        Amount = amount;
        Type = type;
        Description = description;
    }
}